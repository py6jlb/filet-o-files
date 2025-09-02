using CSharpFunctionalExtensions;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Mappings;
using FiletOFiles.Api.Models;

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

    public async Task<Result<GetTagResponse>> AddTag(AddTagRequest request)
    {
        try
        {
            var newTag = request.ToTag();
            await _db.Tags.AddAsync(newTag);
            await _db.SaveChangesAsync();
            var result = newTag.ToResponse();
            return Result.Success(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка создания новой метки");
            return Result.Failure<GetTagResponse>("Ошибка создания новой метки");
        }
    }
}
