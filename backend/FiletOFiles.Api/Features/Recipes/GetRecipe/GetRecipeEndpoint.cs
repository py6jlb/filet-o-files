using System;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.GetRecipe;

public static class GetRecipeEndpoint
{
    public static IEndpointRouteBuilder MapGetRecipe(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGet("/{id}", HandleAsync)
            .WithName(nameof(GetRecipeEndpoint))
            .WithDescription("Получить рецепт по его ID")
            .Produces<RecipeDto>(StatusCodes.Status200OK)
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
        var recipe = await db
            .Recipes.Where(x => x.Id == id)
            .Include(x => x.Tags)
            .Include(x => x.Files)
            .ThenInclude(f => f.PreviewFile)
            .FirstOrDefaultAsync(cancellationToken);

        return recipe is null ? TypedResults.NotFound() : TypedResults.Ok(recipe.ToDto());
    }
}
