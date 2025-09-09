using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.GetTag;

public interface IGetTagHandler
{
    Task<Result<TagDto?>> GetTag(string id, CancellationToken cancellationToken = default);
}
