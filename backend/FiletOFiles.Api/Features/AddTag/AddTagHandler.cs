using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;

namespace FiletOFiles.Api.Features.AddTag;

public class AddTagHandler : IAddTagHandler
{
    private readonly ILogger<AddTagHandler> _logger;
    private readonly AppDbContext _db;

    public AddTagHandler(ILogger<AddTagHandler> logger, AppDbContext db)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Result<TagDto>> AddTag(TagDto request)
    {
        try
        {
            var newTag = request.ToEntity();
            await _db.Tags.AddAsync(newTag);
            await _db.SaveChangesAsync();
            var result = newTag.ToDto();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка создания новой метки");
            return Result.Failure<TagDto>("Ошибка создания новой метки");
        }
    }
}
