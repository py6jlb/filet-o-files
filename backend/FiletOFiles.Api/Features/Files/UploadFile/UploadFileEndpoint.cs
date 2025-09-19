using System;
using System.Security.Cryptography;
using FiletOFiles.Api;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Files.UploadFile;

public static class UploadFileEndpoint
{
    public static IEndpointRouteBuilder MapUploadFile(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("/", HandleAsync)
            .WithName(nameof(UploadFileEndpoint))
            .WithDescription("Загрузить файл")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] AppDbContext db,
        [FromServices] Filestorage cfg,
        [FromBody] DTOs.Files.UploadFile request,
        CancellationToken cancellationToken
    )
    {
        var recipe = await db.Recipes.FirstOrDefaultAsync(
            x => x.Id == request.RecipeId,
            cancellationToken
        );

        if (recipe is null)
        {
            return TypedResults.Problem(
                detail: "Не найден рецепт",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        string sha1Hash;
        using (var stream = request.File.OpenReadStream())
        {
            using var sha1 = SHA1.Create();
            byte[] hashBytes = await sha1.ComputeHashAsync(stream, cancellationToken);
            sha1Hash = Convert.ToHexStringLower(hashBytes);
        }

        var path = Path.Combine(cfg.Path, recipe.Id);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var filePath = Path.Combine(path, sha1Hash);
        if (File.Exists(filePath))
        {
            return TypedResults.Problem(
                detail: $"A file with the name '{request.File.FileName}' already exists.",
                statusCode: StatusCodes.Status409Conflict
            );
        }
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var newFile = new FileDto()
        {
            FileName = request.File.FileName,
            Source = path,
            MimeType = MimeTypes.GetMimeType(request.File.FileName),
            IsTitle = request.IsTitle,
            RecipeId = recipe.Id,
            Size = request.File.Length,
        };
        await db.Files.AddAsync(newFile.ToEntity(), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
