using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.AddTag;

public interface IAddTagHandler
{
    Task<Result<GetTagResponse>> AddTag(AddTagRequest request);
}
