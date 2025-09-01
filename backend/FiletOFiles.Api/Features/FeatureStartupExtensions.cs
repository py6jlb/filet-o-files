using System;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;

namespace FiletOFiles.Api.Features;

public static class FeatureStartupExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddScoped<IGetRecipeHandler, GetRecipeHandler>();
        services.AddScoped<IGetRecipesHandler, GetRecipesHandler>();
        services.AddScoped<IAddRecipeHandler, AddRecipeHandler>();

        return services;
    }
}
