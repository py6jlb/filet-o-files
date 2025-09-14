using System;
using FiletOFiles.Api.DTOs.Tags;
using FluentResults;

namespace FiletOFiles.Api.Features.Tags.AddTag;

public interface IAddTagHandler
{
    Task<Result<TagDto>> AddTag(
        CreateTagDto request,
        CancellationToken cancellationToken = default
    );
}
