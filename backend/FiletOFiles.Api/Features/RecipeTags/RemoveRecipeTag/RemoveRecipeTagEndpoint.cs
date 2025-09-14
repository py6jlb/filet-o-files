using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;

public static class RemoveRecipeTagEndpoint
{
    public static IEndpointRouteBuilder MapRemoveRecipeTag(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapDelete("/{recipeId}/tags/{tagId}", HandleAsync)
            .WithName(nameof(RemoveRecipeTagEndpoint))
            .WithDescription("Удалить метку с рецепта")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string recipeId,
        [FromRoute] string tagId,
        [FromServices] AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var recipeTag = await db.RecipeTag.FirstOrDefaultAsync(
            x => x.RecipeId == recipeId && x.TagId == tagId,
            cancellationToken: cancellationToken
        );

        if (recipeTag is null)
        {
            return TypedResults.Problem(
                detail: "Нет записи для удаления",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        db.RecipeTag.Remove(recipeTag);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
