using System;
using CSharpFunctionalExtensions;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Features.GetTags;

public interface IGetTagsHandler
{
    Task<Result<IReadOnlyCollection<GetTagResponse>>> GetTags(string textFragment);
}
