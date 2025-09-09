using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.Recipes.AddRecipe;

public interface IAddRecipeHandler
{
    Task<Result<RecipeDto>> AddRecipe(
        CreateRecipeDto request,
        CancellationToken cancellationToken = default
    );
}
