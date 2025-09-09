using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.DeleteRecipe;

public class DeleteRecipeHandler : IDeleteRecipeHandler
{
    private readonly ILogger<DeleteRecipeHandler> _logger;
    private readonly AppDbContext _db;

    public DeleteRecipeHandler(ILogger<DeleteRecipeHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Delete(string recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(
            x => x.Id == recipeId,
            cancellationToken: cancellationToken
        );
        if (recipe is null)
        {
            return Result.Failure("Не найден рецепт для удаления");
        }

        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
