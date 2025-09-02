using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Recipes;

namespace FiletOFiles.Api.Features.GetRecipes;

public interface IGetRecipesHandler
{
    Task<Result<IReadOnlyCollection<RecipeDto>>> GetRecipes(RecipeQueryParameters request);
}
