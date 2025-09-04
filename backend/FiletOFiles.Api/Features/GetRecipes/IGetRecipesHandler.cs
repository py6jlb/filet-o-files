using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.GetRecipes;

public interface IGetRecipesHandler
{
    Task<Result<RecipesCollectionDto>> GetRecipes(
        RecipeQueryParameters request,
        CancellationToken cancellationToken = default
    );
}
