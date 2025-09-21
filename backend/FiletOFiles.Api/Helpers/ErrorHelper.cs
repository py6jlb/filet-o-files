using System;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FiletOFiles.Api.Helpers;

public static class ErrorHelper
{
    public static int GetHttpCodeFromError(IError err)
    {
        if (err == null)
        {
            return 200;
        }
        var code = (int?)
            err.Metadata.Where(x => x.Key == "code").Select(x => x.Value).FirstOrDefault();
        return code.HasValue ? code.Value : 400;
    }

    public static Dictionary<string, object?>? GetProblemExtensionsFromError(IError err)
    {
        if (err == null)
        {
            return null;
        }

        if (err.Metadata.TryGetValue("extensions", out var extensions))
        {
            return (Dictionary<string, object?>?)extensions;
        }
        else
        {
            return null;
        }
    }

    public static ProblemHttpResult GetProblem(IError err)
    {
        if (err == null)
        {
            return TypedResults.Problem();
        }
        var p = TypedResults.Problem(
            detail: err.Message,
            statusCode: GetHttpCodeFromError(err),
            extensions: GetProblemExtensionsFromError(err)
        );
        return p;
    }
}
