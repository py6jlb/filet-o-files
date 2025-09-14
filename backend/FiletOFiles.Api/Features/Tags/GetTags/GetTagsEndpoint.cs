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
            .MapGet("", HandleAsync)
            .WithName(nameof(GetTagsEndpoint))
            .Produces<PaginationResult<TagDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        return endpointRouteBuilder;
    }

    public static async Task<IResult> HandleAsync(
        string id,
        [FromServices] AppDbContext db,
        [FromServices] SortMappingProvider sortMappingProvider,
        TagsQueryParameters request,
        CancellationToken cancellationToken
    )
    {
        request.Search ??= request.Search?.Trim().ToLower();
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<TagDto, Tag>();
        IQueryable<TagDto> tagsQuery = db
            .Tags.Where(x => request.Search == null || x.Name.ToLower().Contains(request.Search))
            .ApplySort(request.Sort, sortMappings)
            .Select(r => r.ToDto());
        var tags = await PaginationResult<TagDto>.CreateAsync(
            tagsQuery,
            request.Page,
            request.PageSize,
            cancellationToken
        );

        return tags.TotalCount == 0 ? TypedResults.NotFound() : TypedResults.Ok(tags);
    }
}
