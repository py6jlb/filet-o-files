using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetTags;

public class GetTagsHandler : IGetTagsHandler
{
    private readonly ILogger<GetTagsHandler> _logger;
    private readonly AppDbContext _db;

    public GetTagsHandler(ILogger<GetTagsHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<TagsCollectionDto>> GetTags(
        string textFragment,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(textFragment))
            {
                var result = await _db
                    .Tags.Where(x => EF.Functions.Like(x.Name, $"%{textFragment}%"))
                    .Select(x => x.ToDto())
                    .ToArrayAsync(cancellationToken);
                return Result.Success(new TagsCollectionDto(result ?? []));
            }
            else
            {
                var result = await _db.Tags.Select(x => x.ToDto()).ToArrayAsync(cancellationToken);
                return Result.Success(new TagsCollectionDto(result ?? []));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка поиска меток");
            return Result.Failure<TagsCollectionDto>("Ошибка поиска меток");
        }
    }
}
