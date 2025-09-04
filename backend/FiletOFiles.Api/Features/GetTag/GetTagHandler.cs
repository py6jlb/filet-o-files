using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.GetTag;

public class GetTagHandler : IGetTagHandler
{
    private readonly ILogger<GetTagHandler> _logger;
    private readonly AppDbContext _db;

    public GetTagHandler(ILogger<GetTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<TagDto?>> GetTag(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return Result.Success(tag?.ToDto());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка получения метки");
            return Result.Failure<TagDto?>("Ошибка получения метки");
        }
    }
}
