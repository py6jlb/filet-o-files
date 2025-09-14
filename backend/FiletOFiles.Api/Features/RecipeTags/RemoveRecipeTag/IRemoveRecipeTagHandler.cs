using System;
using FluentResults;

namespace FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;

public interface IRemoveRecipeTagHandler
{
    Task<Result> Remove(
        string recipeId,
        string tagId,
        CancellationToken cancellationToken = default
    );
}
