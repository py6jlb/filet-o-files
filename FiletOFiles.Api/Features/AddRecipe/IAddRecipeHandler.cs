using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;

namespace FiletOFiles.Api.Features.AddRecipe;

internal interface IAddRecipeHandler
{
    Task<Result<Recipe>> GetRecipe(long id);

    Task<Result<IReadOnlyCollection<Recipe>>> GetRecipes(long take, long skip);
}
