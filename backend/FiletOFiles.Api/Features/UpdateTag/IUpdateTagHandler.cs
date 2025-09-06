using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.UpdateTag;

public interface IUpdateTagHandler
{
    Task<Result> Update(
        string id,
        UpdateTagDto request,
        CancellationToken cancellationToken = default
    );
}
