using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Files.GetFile;

public static class GetFileEndpoint
{
    public static IEndpointRouteBuilder MapGetFile(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/", HandleAsync)
            .WithName(nameof(GetFileEndpoint))
            .WithDescription("Удалить файл")
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
