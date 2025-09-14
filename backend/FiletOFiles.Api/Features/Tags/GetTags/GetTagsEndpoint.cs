using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services.Sorting;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.Features.Tags.GetTags;

public static class GetTagsEndpoint
{
    public static IEndpointRouteBuilder MapGetTags(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/", HandleAsync)
            .WithName(nameof(GetTagsEndpoint))
            .WithDescription("Получить метки")
            .Produces<PaginationResult<TagDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        [FromQuery(Name = "q")] string? Search,
        [FromQuery(Name = "sort")] string? Sort,
        [FromServices] AppDbContext db,
        [FromServices] SortMappingProvider sortMappingProvider,
        CancellationToken cancellationToken,
        [FromQuery(Name = "page")] int Page = 1,
        [FromQuery(Name = "pageSize")] int PageSize = 10
    )
    {
        Search ??= Search?.Trim().ToLower();
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<TagDto, Tag>();
        IQueryable<TagDto> tagsQuery = db
            .Tags.Where(x => Search == null || x.Name.ToLower().Contains(Search))
            .ApplySort(Sort, sortMappings)
            .Select(r => r.ToDto());
        var tags = await PaginationResult<TagDto>.CreateAsync(
            tagsQuery,
            Page,
            PageSize,
            cancellationToken
        );

        return tags.TotalCount == 0 ? TypedResults.NotFound() : TypedResults.Ok(tags);
    }
}
