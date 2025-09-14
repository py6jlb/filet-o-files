using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Features.Tags.AddTag;

public class AddTagHandler : IAddTagHandler
{
    private readonly ILogger<AddTagHandler> _logger;
    private readonly AppDbContext _db;

    public AddTagHandler(ILogger<AddTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<TagDto>> AddTag(
        CreateTagDto request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var tag = request.ToEntity();
            var exists = await _db.Tags.AnyAsync(x => x.Name == tag.Name, cancellationToken);
            if (exists)
            {
                return Result.Fail<TagDto>($"Метка с названием '{tag.Name}' уже существует");
            }
            await _db.Tags.AddAsync(tag, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            var result = tag.ToDto();
            return Result.Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка создания новой метки");
            return Result.Fail<TagDto>("Ошибка создания новой метки");
        }
    }
}
