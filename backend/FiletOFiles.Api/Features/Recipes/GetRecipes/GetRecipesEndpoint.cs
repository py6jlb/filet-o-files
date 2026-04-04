using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services.Sorting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Recipes.GetRecipes;

public static class GetRecipesEndpoint
{
    public static IEndpointRouteBuilder MapGetRecipes(
        this IEndpointRouteBuilder endpointRouteBuilder
    )
    {
        endpointRouteBuilder
            .MapGet("/", HandleAsync)
            .WithName(nameof(GetRecipesEndpoint))
            .WithDescription("Получить рецепты")
            .Produces<PaginationResult<RecipeDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery(Name = "q")] string? Search,
        [FromQuery(Name = "sort")] string? Sort,
        [FromQuery(Name = "tags")] string? Tags,
        [FromQuery(Name = "fields")] string? Fields,
        [FromServices] AppDbContext db,
        [FromServices] SortMappingProvider sortMappingProvider,
        CancellationToken cancellationToken,
        [FromQuery(Name = "page")] int Page = 1,
        [FromQuery(Name = "pageSize")] int PageSize = 10
    )
    {
        if (!sortMappingProvider.ValidateMappings<RecipeDto, Recipe>(Sort))
        {
            return TypedResults.Problem(
                detail: $"The provided sort parameter isn't valid: '{Sort}'",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        Search ??= Search?.Trim().ToLower();
        var sortMappings = sortMappingProvider.GetMappings<RecipeDto, Recipe>();

        IQueryable<Recipe> recipesQuery = db
            .Recipes.Include(r => r.Tags)
            .Include(r => r.RecipeTags)
            .Include(r => r.Files)
            .ThenInclude(f => f.PreviewFile)
            .AsQueryable();

        // Фильтрация по названию/описанию
        if (!string.IsNullOrEmpty(Search))
        {
            recipesQuery = recipesQuery.Where(r =>
                r.Title.ToLower().Contains(Search)
                || r.Descriptions != null && r.Descriptions.ToLower().Contains(Search)
            );
        }

        // Фильтрация по тегам
        if (!string.IsNullOrEmpty(Tags))
        {
            var tagIds = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (tagIds.Length > 0)
            {
                recipesQuery = recipesQuery.Where(r => r.Tags.Any(t => tagIds.Contains(t.Id)));
            }
        }

        var result = await PaginationResult<RecipeDto>.CreateAsync(
            recipesQuery.ApplySort(Sort, sortMappings).Select(r => r.ToDto()),
            Page,
            PageSize,
            cancellationToken
        );

        return TypedResults.Ok(result);
    }
}
