using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.Recipes.UpdateRecipe;

public interface IUpdateRecipeHandler
{
    Task<Result> Update(
        string id,
        UpdateRecipeDto request,
        CancellationToken cancellationToken = default
    );
}
