using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Mappings;
using FiletOFiles.Api.Models;
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

    public async Task<Result<GetRecipeResponse>> AddRecipe(AddRecipeRequest request)
    {
        try
        {
            var newRecipe = request.ToRecipe();
            await _db.Recipes.AddAsync(newRecipe);
            await _db.SaveChangesAsync();
            var result = newRecipe.ToResponse();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка добавления нового рецепта");
            return Result.Failure<GetRecipeResponse>("Ошибка добавления нового рецепта");
        }
    }
}
