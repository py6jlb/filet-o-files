using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Features.DeleteTag;

public interface IDeleteTagHandler
{
    Task<Result> Delete(string id, CancellationToken cancellationToken = default);
}
