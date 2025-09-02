using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.GetRecipes;

public interface IGetRecipesHandler
{
    Task<Result<IReadOnlyCollection<GetRecipeResponse>>> GetRecipes(GetRecipesRequest request);
}
