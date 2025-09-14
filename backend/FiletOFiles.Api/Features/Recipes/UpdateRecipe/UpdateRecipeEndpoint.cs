using System;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.UpdateRecipe;

public static class UpdateRecipeEndpoint
{
    public static IEndpointRouteBuilder MapUpdateRecipe(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapDelete("{id}", HandleAsync)
            .WithName(nameof(UpdateRecipeEndpoint))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        string id,
        UpdateRecipeDto request,
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
                detail: "Не найден рецепт",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        recipe.UpdateFromDto(request);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
