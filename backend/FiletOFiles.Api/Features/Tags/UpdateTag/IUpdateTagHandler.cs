using System;
using FiletOFiles.Api.DTOs.Tags;
using FluentResults;

namespace FiletOFiles.Api.Features.Tags.UpdateTag;

public interface IUpdateTagHandler
{
    Task<Result> Update(
        string id,
        UpdateTagDto request,
        CancellationToken cancellationToken = default
    );
}
