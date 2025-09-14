using System;
using FiletOFiles.Api.DTOs.Tags;
using FluentResults;

namespace FiletOFiles.Api.Features.Tags.GetTag;

public interface IGetTagHandler
{
    Task<Result<TagDto?>> GetTag(string id, CancellationToken cancellationToken = default);
}
