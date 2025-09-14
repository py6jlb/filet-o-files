using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.RecipeTag;
using FiletOFiles.Api.Infrastructure.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.RemoveRecipeTag.UpsertRecipeTags;

public class UpsertRecipeTagsHandler : IUpsertRecipeTagsHandler
{
    private readonly ILogger<UpsertRecipeTagsHandler> _logger;
    private readonly AppDbContext _db;

    public UpsertRecipeTagsHandler(ILogger<UpsertRecipeTagsHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Upsert(
        string recipeId,
        UpsertRecipeTagsDto request,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await _db
            .Recipes.Include(r => r.RecipeTags)
            .FirstOrDefaultAsync(r => r.Id == recipeId, cancellationToken: cancellationToken);

        if (recipe is null)
        {
            return Result.Fail("Рецепт не найден");
        }

        var currentTagIds = recipe.RecipeTags.Select(x => x.TagId).ToHashSet();
        if (currentTagIds.SetEquals(request.TagIds))
        {
            return Result.Ok();
        }

        List<string> existingTagIds = await _db
            .Tags.Where(t => request.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        if (existingTagIds.Count != request.TagIds.Count)
        {
            return Result.Fail("One or more tag IDs is invalid");
        }

        recipe.RecipeTags.RemoveAll(rt => !request.TagIds.Contains(rt.TagId));
        var tagIdsToAdd = request.TagIds.Except(currentTagIds).ToArray();
        recipe.RecipeTags.AddRange(
            tagIdsToAdd.Select(tagId => new RecipeTag { RecipeId = recipeId, TagId = tagId })
        );

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
