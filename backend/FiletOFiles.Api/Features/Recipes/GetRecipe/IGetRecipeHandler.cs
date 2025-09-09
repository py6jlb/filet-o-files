using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.Recipes.GetRecipe;

public interface IGetRecipeHandler
{
    Task<Result<RecipeDto?>> GetRecipe(string id, CancellationToken cancellationToken = default);
}
