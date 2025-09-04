using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.GetTags;

public interface IGetTagsHandler
{
    Task<Result<TagsCollectionDto>> GetTags(
        string textFragment,
        CancellationToken cancellationToken = default
    );
}
