using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Files.DeleteFile;

public static class DeleteFileEndpoint
{
    public static IEndpointRouteBuilder MapDeleteFile(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapDelete("/", HandleAsync)
            .WithName(nameof(DeleteFileEndpoint))
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
