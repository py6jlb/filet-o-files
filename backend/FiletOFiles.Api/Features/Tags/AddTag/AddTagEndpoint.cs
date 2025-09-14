using System;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.AddTag;

public static class AddTagEndpoint
{
    public static IEndpointRouteBuilder MapAddTag(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("", HandleAsync)
            .WithName(nameof(AddTagEndpoint))
            .Produces<TagDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status409Conflict)
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
        var tag = request.ToEntity();
        var exists = await db.Tags.AnyAsync(x => x.Name == tag.Name, cancellationToken);
        if (exists)
        {
            return TypedResults.Problem(
                detail: $"Метка с названием '{tag.Name}' уже существует",
                statusCode: StatusCodes.Status409Conflict
            );
        }

        await db.Tags.AddAsync(tag, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        var result = tag.ToDto();

        return TypedResults.Ok(result);
    }
}
