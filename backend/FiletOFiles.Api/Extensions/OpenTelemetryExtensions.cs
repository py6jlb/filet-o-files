using System;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FiletOFiles.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
            .WithTracing(t => t.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation())
            .WithMetrics(m =>
                m.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
            );

        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;
        });

        return builder;
    }
}
