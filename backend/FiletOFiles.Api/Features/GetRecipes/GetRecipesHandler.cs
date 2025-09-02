using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
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

    public async Task<Result<IReadOnlyCollection<RecipeDto>>> GetRecipes(
        RecipeQueryParameters request
    )
    {
        try
        {
            var recipes = await _db
                .Recipes.Where(x => EF.Functions.Like(x.Title, $"%{request.Search}%"))
                .Include(x => x.Tags)
                .Include(x => x.Files)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(x => x.ToDto())
                .ToArrayAsync();
            IReadOnlyCollection<RecipeDto> result = recipes ?? [];
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка получения списка рецептов");
            return Result.Failure<IReadOnlyCollection<RecipeDto>>(
                "Ошибка получения списка рецептов"
            );
        }
    }
}
