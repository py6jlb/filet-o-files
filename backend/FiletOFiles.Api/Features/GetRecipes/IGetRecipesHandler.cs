using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.GetRecipes;

internal interface IGetRecipesHandler
{
    Task<Result<IReadOnlyCollection<GetRecipeResponse>>> GetRecipes(int take, int skip);
}
