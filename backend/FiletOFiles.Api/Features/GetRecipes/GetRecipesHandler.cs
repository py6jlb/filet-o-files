using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Mappings;
using FiletOFiles.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetRecipes;

public sealed class GetRecipesHandler : IGetRecipesHandler
{
    private readonly ILogger<GetRecipesHandler> _logger;
    private readonly AppDbContext _db;

    public GetRecipesHandler(ILogger<GetRecipesHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<GetRecipeResponse>>> GetRecipes(
        GetRecipesRequest request
    )
    {
        try
        {
            var recipes = await _db
                .Recipes.Include(x => x.Tags)
                .Include(x => x.Files)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(x => x.ToResponse())
                .ToArrayAsync();
            IReadOnlyCollection<GetRecipeResponse> result = recipes ?? [];
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка получения списка рецептов");
            return Result.Failure<IReadOnlyCollection<GetRecipeResponse>>(
                "Ошибка получения списка рецептов"
            );
        }
    }
}
