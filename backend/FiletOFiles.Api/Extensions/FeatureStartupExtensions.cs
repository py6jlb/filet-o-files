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

namespace FiletOFiles.Api.Extensions;

public static class FeatureStartupExtensions
{
    public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IGetRecipeHandler, GetRecipeHandler>();
        builder.Services.AddScoped<IGetRecipesHandler, GetRecipesHandler>();
        builder.Services.AddScoped<IAddRecipeHandler, AddRecipeHandler>();
        builder.Services.AddScoped<IUpdateRecipeHandler, UpdateRecipeHandler>();

        builder.Services.AddScoped<IAddTagHandler, AddTagHandler>();
        builder.Services.AddScoped<IGetTagHandler, GetTagHandler>();
        builder.Services.AddScoped<IGetTagsHandler, GetTagsHandler>();
        builder.Services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        builder.Services.AddScoped<IUpdateTagHandler, UpdateTagHandler>();

        builder.Services.AddScoped<IRemoveRecipeTagHandler, RemoveRecipeTagHandler>();
        builder.Services.AddScoped<IUpsertRecipeTagsHandler, UpsertRecipeTagsHandler>();

        return builder;
    }
}
