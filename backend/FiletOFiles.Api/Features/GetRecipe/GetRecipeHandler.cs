using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure;
using FiletOFiles.Api.Mappings;
using FiletOFiles.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetRecipe;

internal sealed class GetRecipeHandler : IGetRecipeHandler
{
    private readonly ILogger<GetRecipeHandler> _logger;
    private readonly AppDbContext _db;

    public GetRecipeHandler(ILogger<GetRecipeHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<GetRecipeResponse?>> GetRecipe(long id)
    {
        try
        {
            var recipe = await _db
                .Recipes.Where(x => x.Id == id)
                .Include(x => x.Tags)
                .Include(x => x.Files)
                .FirstOrDefaultAsync();
            return Result.Success(recipe?.ToResponse());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка получения рецепта");
            return Result.Failure<GetRecipeResponse?>("Ошибка получения рецепта");
        }
    }
}
