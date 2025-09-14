using System;
using FiletOFiles.Api.Middleware;

namespace FiletOFiles.Api.Extensions;

public static class ErrorHandlingExtensions
{
    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd(
                    "requestId",
                    context.HttpContext.TraceIdentifier
                );
            };
        });

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        return builder;
    }
}
