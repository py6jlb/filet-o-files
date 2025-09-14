using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.DeleteTag;

public static class DeleteTagEndpoint
{
    public static IEndpointRouteBuilder MapDeleteTag(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapDelete("{id}", HandleAsync)
            .WithName(nameof(DeleteTagEndpoint))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        string id,
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
                detail: "Не найдена метка для удаления",
                statusCode: StatusCodes.Status404NotFound
            );
        }
        db.Tags.Remove(tag);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
