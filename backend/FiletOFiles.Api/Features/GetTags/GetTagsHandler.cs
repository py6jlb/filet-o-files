using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Infrastructure;
using FiletOFiles.Api.Mappings;
using FiletOFiles.Api.Models;
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

    public async Task<Result<IReadOnlyCollection<GetTagResponse>>> GetTags(string textFragment)
    {
        try
        {
            IReadOnlyCollection<GetTagResponse> result = await _db
                .Tags.Where(x => EF.Functions.Like(x.Name, $"%{textFragment}%"))
                .Select(x => x.ToResponse())
                .ToArrayAsync();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка поиска меток");
            return Result.Failure<IReadOnlyCollection<GetTagResponse>>("Ошибка поиска меток");
        }
    }
}
