using System;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Tags.AddTag;

public static class AddTagEndpoint
{
    public static IEndpointRouteBuilder MapAddTag(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("", HandleAsync)
            .WithName(nameof(MapAddTag))
            .Produces<TagDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        CreateTagDto request,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        return TypedResults.Ok();
    }
}
