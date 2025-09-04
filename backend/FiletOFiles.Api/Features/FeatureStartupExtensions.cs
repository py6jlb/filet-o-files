using System;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.AddTag;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;
using FiletOFiles.Api.Features.GetTags;
using FiletOFiles.Api.Features.UpdateRecipe;

namespace FiletOFiles.Api.Features;

public static class FeatureStartupExtensions
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddScoped<IGetRecipeHandler, GetRecipeHandler>();
        services.AddScoped<IGetRecipesHandler, GetRecipesHandler>();
        services.AddScoped<IAddRecipeHandler, AddRecipeHandler>();
        services.AddScoped<IUpdateRecipeHandler, UpdateRecipeHandler>();

        services.AddScoped<IAddTagHandler, AddTagHandler>();
        services.AddScoped<IGetTagsHandler, GetTagsHandler>();

        return services;
    }
}
