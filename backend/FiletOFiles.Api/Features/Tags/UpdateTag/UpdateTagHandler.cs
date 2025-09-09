using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.UpdateTag;

public class UpdateTagHandler : IUpdateTagHandler
{
    private readonly ILogger<UpdateTagHandler> _logger;
    private readonly AppDbContext _db;

    public UpdateTagHandler(ILogger<UpdateTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Update(
        string id,
        UpdateTagDto request,
        CancellationToken cancellationToken = default
    )
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken
        );
        if (tag is null)
        {
            return Result.Failure("Не найдена метка для обновления");
        }

        tag.UpdateFromDto(request);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
