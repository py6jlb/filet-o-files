using System;
using System.Security.Cryptography;
using FiletOFiles.Api;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
        [FromServices] FilesService service,
        [FromBody] DTOs.Files.UploadFile request,
        CancellationToken cancellationToken
    )
    {
        var result = await service.Upload(request, cancellationToken);
        return TypedResults.Ok();
    }
}
