using System;
using FiletOFiles.Api.DTOs.RecipeTag;
using FluentResults;

namespace FiletOFiles.Api.Features.RemoveRecipeTag.UpsertRecipeTags;

public interface IUpsertRecipeTagsHandler
{
    Task<Result> Upsert(
        string recipeId,
        UpsertRecipeTagsDto request,
        CancellationToken cancellationToken = default
    );
}
