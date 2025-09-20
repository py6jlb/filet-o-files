using System;
using System.Security.Cryptography;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Files;

public sealed class FilesService
{
    private readonly AppDbContext _db;
    private readonly Filestorage _cfg;

    public FilesService(AppDbContext db, Filestorage cfg)
    {
        _cfg = cfg;
        _db = db;
    }

    public async Task<Result> Upload(
        DTOs.Files.UploadFile request,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(
            x => x.Id == request.RecipeId,
            cancellationToken
        );

        if (recipe is null)
        {
            return Result.Fail(new Error($"Не найден рецепт").WithMetadata("code", 404));
        }

        string sha1Hash;
        using (var stream = request.File.OpenReadStream())
        {
            using var sha1 = SHA1.Create();
            byte[] hashBytes = await sha1.ComputeHashAsync(stream, cancellationToken);
            sha1Hash = Convert.ToHexStringLower(hashBytes);
        }

        var path = Path.Combine(_cfg.Path, recipe.Id);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var filePath = Path.Combine(path, sha1Hash);
        if (File.Exists(filePath))
        {
            return Result.Fail(
                new Error(
                    $"A file with the name '{request.File.FileName}' already exists."
                ).WithMetadata("code", 409)
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
        await _db.Files.AddAsync(newFile.ToEntity(), cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
