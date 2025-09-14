using System;
using FiletOFiles.Api.Features.Recipes.AddRecipe;
using FiletOFiles.Api.Features.Recipes.DeleteRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipes;
using FiletOFiles.Api.Features.Recipes.UpdateRecipe;

namespace FiletOFiles.Api.Features.Recipes;

public static class RecipeGroup
{
    public static IEndpointRouteBuilder MapRecipesGroup(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGroup("/recipes")
            .WithOpenApi()
            .WithTags("Recipes")
            .RequireAuthorization()
            .MapGetRecipe()
            .MapGetRecipes()
            .MapAddRecipe()
            .MapDeleteRecipe()
            .MapUpdateRecipe();

        return endpointRouteBuilder;
    }
}
