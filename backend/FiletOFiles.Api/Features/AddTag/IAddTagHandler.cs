using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.Features.AddTag;

public interface IAddTagHandler
{
    Task<Result<TagDto>> AddTag(TagDto request);
}
