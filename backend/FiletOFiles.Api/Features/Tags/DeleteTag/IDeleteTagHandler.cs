using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Features.Tags.DeleteTag;

public interface IDeleteTagHandler
{
    Task<Result> Delete(string id, CancellationToken cancellationToken = default);
}
