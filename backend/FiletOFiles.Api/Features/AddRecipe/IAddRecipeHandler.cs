using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.AddRecipe;

internal interface IAddRecipeHandler
{
    Task<Result> AddRecipe(AddRecipeRequest request);
}
