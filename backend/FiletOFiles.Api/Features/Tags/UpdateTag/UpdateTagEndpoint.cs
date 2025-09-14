using System;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.UpdateTag;

public static class UpdateTagEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTag(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapPost("/{id}", HandleAsync)
            .WithName(nameof(UpdateTagEndpoint))
            .WithDescription("Обновить метку")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string id,
        [FromBody] UpdateTagDto request,
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
                detail: "Не найдена метка для обновления",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // bool tagWithNameExists = await db.Tags.AnyAsync(
        //     x => x.Id != id && x.Name.ToLower() == request.Name.ToLower(),
        //     cancellationToken
        // );

        // if (tagWithNameExists)
        // {
        //     return TypedResults.Problem(
        //         detail: $"The tag '{request.Name}' already exists",
        //         statusCode: StatusCodes.Status409Conflict
        //     );
        // }

        tag.UpdateFromDto(request);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
