using System;
using FiletOFiles.Api.Features.AddRecipe;
using FiletOFiles.Api.Features.AddTag;
using FiletOFiles.Api.Features.DeleteTag;
using FiletOFiles.Api.Features.GetRecipe;
using FiletOFiles.Api.Features.GetRecipes;
using FiletOFiles.Api.Features.GetTag;
using FiletOFiles.Api.Features.GetTags;
using FiletOFiles.Api.Features.RemoveRecipeTag;
using FiletOFiles.Api.Features.UpdateRecipe;
using FiletOFiles.Api.Features.UpdateTag;
using FiletOFiles.Api.Features.UpsertRecipeTags;

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
        services.AddScoped<IGetTagHandler, GetTagHandler>();
        services.AddScoped<IGetTagsHandler, GetTagsHandler>();
        services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        services.AddScoped<IUpdateTagHandler, UpdateTagHandler>();

        services.AddScoped<IRemoveRecipeTagHandler, RemoveRecipeTagHandler>();
        services.AddScoped<IUpsertRecipeTagsHandler, UpsertRecipeTagsHandler>();

        return services;
    }
}
