using System;
using FiletOFiles.Api.Features.AddRecipe;

namespace FiletOFiles.Api.Features;

public static class FeatureStartupExtensions
{
    public static IServiceCollection AddFeatures(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAddRecipeHandler, AddRecipeHandler>();
        return services;
    }
}
