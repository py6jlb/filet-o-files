using System;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.GetTag;

public static class GetTagEndpoint
{
    public static IEndpointRouteBuilder MapGetTag(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/{id}", HandleAsync)
            .WithName(nameof(GetTagEndpoint))
            .WithDescription("ПОлучить метку")
            .Produces<TagDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string id,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var tag = await db.Tags.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken
        );

        if (tag is null)
        {
            return TypedResults.Problem(
                detail: "Не найдена метка",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        return TypedResults.Ok(tag.ToDto());
    }
}
