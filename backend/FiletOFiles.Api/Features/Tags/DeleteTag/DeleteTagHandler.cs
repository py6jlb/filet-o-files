using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.DeleteTag;

public class DeleteTagHandler : IDeleteTagHandler
{
    private readonly ILogger<DeleteTagHandler> _logger;
    private readonly AppDbContext _db;

    public DeleteTagHandler(ILogger<DeleteTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result> Delete(string id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken: cancellationToken
        );
        if (tag is null)
        {
            return Result.Failure("Не найдена метка для удаления");
        }

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
