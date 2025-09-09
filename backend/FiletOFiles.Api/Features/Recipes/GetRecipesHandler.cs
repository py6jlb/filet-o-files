using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetRecipes;

public sealed class GetRecipesHandler : IGetRecipesHandler
{
    private readonly ILogger<GetRecipesHandler> _logger;
    private readonly AppDbContext _db;
    private readonly SortMappingProvider _sortMappingProvider;

    public GetRecipesHandler(
        ILogger<GetRecipesHandler> logger,
        AppDbContext db,
        SortMappingProvider sortMappingProvider
    )
    {
        _db = db;
        _logger = logger;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<Result<PaginationResult<RecipeDto>>> GetRecipes(
        RecipeQueryParameters request,
        CancellationToken cancellationToken = default
    )
    {
        if (!_sortMappingProvider.ValidateMappings<RecipeDto, Recipe>(request.Sort))
        {
            return Result.Failure<PaginationResult<RecipeDto>>(
                $"The provided sort parameter isn't valid: '{request.Sort}'"
            );
        }

        try
        {
            request.Search ??= request.Search?.Trim().ToLower();
            SortMapping[] sortMappings = _sortMappingProvider.GetMappings<RecipeDto, Recipe>();
            IQueryable<RecipeDto> recipesQuery = _db
                // .Recipes.Where(r =>
                //     request.Search == null
                //     || EF.Functions.Like(r.Title.ToLower(), $"%{request.Search}%")
                //     || r.Descriptions != null
                //         && EF.Functions.Like(r.Descriptions.ToLower(), $"%{request.Search}%")
                // )
                .Recipes.Where(r =>
                    request.Search == null
                    || r.Title.ToLower().Contains(request.Search)
                    || r.Descriptions != null && r.Descriptions.ToLower().Contains(request.Search)
                )
                .ApplySort(request.Sort, sortMappings)
                .Select(r => r.ToDto());

            var result = await PaginationResult<RecipeDto>.CreateAsync(
                recipesQuery,
                request.Page,
                request.PageSize,
                cancellationToken
            );
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка получения списка рецептов");
            return Result.Failure<PaginationResult<RecipeDto>>("Ошибка получения списка рецептов");
        }
    }
}
