using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.UpdateRecipe;

public class UpdateRecipeHandler : IUpdateRecipeHandler
{
    private readonly ILogger<UpdateRecipeHandler> _logger;
    private readonly AppDbContext _db;

    public UpdateRecipeHandler(ILogger<UpdateRecipeHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Update(
        string id,
        UpdateRecipeDto request,
        CancellationToken cancellationToken = default
    )
    {
        var recipe = await _db.Recipes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure("Рецепт не найден");
        }

        recipe.UpdateFromDto(request);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
