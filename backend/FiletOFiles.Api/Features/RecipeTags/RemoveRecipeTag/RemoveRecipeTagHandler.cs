using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;

public class RemoveRecipeTagHandler : IRemoveRecipeTagHandler
{
    private readonly ILogger<RemoveRecipeTagHandler> _logger;
    private readonly AppDbContext _db;

    public RemoveRecipeTagHandler(ILogger<RemoveRecipeTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Remove(
        string recipeId,
        string tagId,
        CancellationToken cancellationToken = default
    )
    {
        var recipeTag = await _db.RecipeTag.FirstOrDefaultAsync(
            x => x.RecipeId == recipeId && x.TagId == tagId,
            cancellationToken: cancellationToken
        );

        if (recipeTag is null)
        {
            return Result.Failure("Нет записи для удаления");
        }

        _db.RecipeTag.Remove(recipeTag);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
