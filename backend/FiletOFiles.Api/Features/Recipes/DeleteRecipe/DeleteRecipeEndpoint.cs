using System;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.DeleteRecipe;

public static class DeleteRecipeEndpoint
{
    public static IEndpointRouteBuilder MapDeleteRecipe(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapDelete("/{id}", HandleAsync)
            .WithName(nameof(DeleteRecipeEndpoint))
            .WithDescription("Удалить рецепт")
            .Produces(StatusCodes.Status204NoContent)
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
        var recipe = await db.Recipes.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken
        );

        if (recipe is null)
        {
            return TypedResults.Problem(
                detail: "Не найден рецепт для удаления",
                statusCode: StatusCodes.Status404NotFound
            );
        }
        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
