using System;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FiletOFiles.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
            .WithTracing(t => t.AddHttpClientInstrumentation().AddAspNetCoreInstrumentation())
            .WithMetrics(m =>
                m.AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
            )
            .UseOtlpExporter();

        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;
        });
        
        return services;
    }
}
