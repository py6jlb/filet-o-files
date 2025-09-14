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
            .MapGet("", HandleAsync)
            .WithName(nameof(GetRecipesEndpoint))
            .Produces<PaginationResult<RecipeDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        string id,
        [FromServices] AppDbContext db,
        [FromServices] SortMappingProvider sortMappingProvider,
        RecipeQueryParameters request,
        CancellationToken cancellationToken
    )
    {
        if (!sortMappingProvider.ValidateMappings<RecipeDto, Recipe>(request.Sort))
        {
            return TypedResults.Problem(
                detail: $"The provided sort parameter isn't valid: '{request.Sort}'",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        request.Search ??= request.Search?.Trim().ToLower();
        var sortMappings = sortMappingProvider.GetMappings<RecipeDto, Recipe>();
        var recipesQuery = db
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

        return result.TotalCount == 0 ? TypedResults.NotFound() : TypedResults.Ok(result);
    }
}
