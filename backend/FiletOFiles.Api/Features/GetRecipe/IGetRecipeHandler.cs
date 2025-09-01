using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.GetRecipe;

internal interface IGetRecipeHandler
{
    Task<Result<GetRecipeResponse?>> GetRecipe(long id);
}
