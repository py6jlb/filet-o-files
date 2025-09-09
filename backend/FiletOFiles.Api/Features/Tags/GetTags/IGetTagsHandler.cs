using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.Tags.GetTags;

public interface IGetTagsHandler
{
    Task<Result<PaginationResult<TagDto>>> GetTags(
        TagsQueryParameters request,
        CancellationToken cancellationToken = default
    );
}
