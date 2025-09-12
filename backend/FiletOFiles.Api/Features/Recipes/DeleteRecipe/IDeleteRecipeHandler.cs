using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Features.DeleteRecipe;

public interface IDeleteRecipeHandler
{
    Task<Result> Delete(string recipeId, CancellationToken cancellationToken = default);
}
