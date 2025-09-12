using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services.Sorting;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetTags;

public class GetTagsHandler : IGetTagsHandler
{
    private readonly ILogger<GetTagsHandler> _logger;
    private readonly AppDbContext _db;

    private readonly SortMappingProvider _sortMappingProvider;

    public GetTagsHandler(
        ILogger<GetTagsHandler> logger,
        AppDbContext db,
        SortMappingProvider sortMappingProvider
    )
    {
        _db = db;
        _logger = logger;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<Result<PaginationResult<TagDto>>> GetTags(
        TagsQueryParameters request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            request.Search ??= request.Search?.Trim().ToLower();
            SortMapping[] sortMappings = _sortMappingProvider.GetMappings<TagDto, Tag>();
            IQueryable<TagDto> tagsQuery = _db
                .Tags.Where(x =>
                    request.Search == null || x.Name.ToLower().Contains(request.Search)
                )
                .ApplySort(request.Sort, sortMappings)
                .Select(r => r.ToDto());

            var tags = await PaginationResult<TagDto>.CreateAsync(
                tagsQuery,
                request.Page,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(tags);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка поиска меток");
            return Result.Failure<PaginationResult<TagDto>>("Ошибка поиска меток");
        }
    }
}
