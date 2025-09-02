using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.AddRecipe;

public sealed class AddRecipeHandler : IAddRecipeHandler
{
    private readonly ILogger<AddRecipeHandler> _logger;
    private readonly AppDbContext _db;

    public AddRecipeHandler(ILogger<AddRecipeHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<RecipeDto>> AddRecipe(RecipeDto request)
    {
        try
        {
            var newRecipe = request.ToEntity();
            await _db.Recipes.AddAsync(newRecipe);
            await _db.SaveChangesAsync();
            var result = newRecipe.ToDto();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка добавления нового рецепта");
            return Result.Failure<RecipeDto>("Ошибка добавления нового рецепта");
        }
    }
}
