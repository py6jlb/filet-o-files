using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Features.RemoveRecipeTag;

public interface IRemoveRecipeTagHandler
{
    Task<Result> Remove(
        string recipeId,
        string tagId,
        CancellationToken cancellationToken = default
    );
}
