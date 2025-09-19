using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

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
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok();
    }
}
