using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.RecipeTag;

namespace FiletOFiles.Api.Features.RecipeTags.UpsertRecipeTags;

public interface IUpsertRecipeTagsHandler
{
    Task<Result> Upsert(
        string recipeId,
        UpsertRecipeTagsDto request,
        CancellationToken cancellationToken = default
    );
}
