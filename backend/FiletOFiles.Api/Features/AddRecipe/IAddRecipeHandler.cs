using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.AddRecipe;

public interface IAddRecipeHandler
{
    Task<Result<GetRecipeResponse>> AddRecipe(AddRecipeRequest request);
}
