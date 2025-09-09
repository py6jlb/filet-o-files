using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.Tags.AddTag;

public interface IAddTagHandler
{
    Task<Result<TagDto>> AddTag(
        CreateTagDto request,
        CancellationToken cancellationToken = default
    );
}
