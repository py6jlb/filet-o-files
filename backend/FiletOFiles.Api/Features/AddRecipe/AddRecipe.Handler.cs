using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;

namespace FiletOFiles.Api.Features.AddRecipe;

internal sealed class AddRecipeHandler : IAddRecipeHandler
{
    public Task<Result<Recipe>> GetRecipe(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IReadOnlyCollection<Recipe>>> GetRecipes(long take, long skip)
    {
        throw new NotImplementedException();
    }
}
