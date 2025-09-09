using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.Recipes.GetRecipes;

public interface IGetRecipesHandler
{
    Task<Result<PaginationResult<RecipeDto>>> GetRecipes(
        RecipeQueryParameters request,
        CancellationToken cancellationToken = default
    );
}
