using FiletOFiles.Api.Features.Auth;
using FiletOFiles.Api.Features.Recipes;
using FiletOFiles.Api.Features.Recipes.AddRecipe;
using FiletOFiles.Api.Features.Recipes.DeleteRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipes;
using FiletOFiles.Api.Features.Recipes.UpdateRecipe;
using FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;
using FiletOFiles.Api.Features.RemoveRecipeTag.UpsertRecipeTags;
using FiletOFiles.Api.Features.Tags;
using FiletOFiles.Api.Features.Tags.AddTag;
using FiletOFiles.Api.Features.Tags.DeleteTag;
using FiletOFiles.Api.Features.Tags.GetTag;
using FiletOFiles.Api.Features.Tags.GetTags;
using FiletOFiles.Api.Features.Tags.UpdateTag;
using FiletOFiles.Api.Features.Users;
using FiletOFiles.Api.Features.Users.GetUser;

namespace FiletOFiles.Api.Extensions;

public static class FeatureStartupExtensions
{
    public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAddTagHandler, AddTagHandler>();
        builder.Services.AddScoped<IGetTagHandler, GetTagHandler>();
        builder.Services.AddScoped<IGetTagsHandler, GetTagsHandler>();
        builder.Services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        builder.Services.AddScoped<IUpdateTagHandler, UpdateTagHandler>();

        builder.Services.AddScoped<IRemoveRecipeTagHandler, RemoveRecipeTagHandler>();
        builder.Services.AddScoped<IUpsertRecipeTagsHandler, UpsertRecipeTagsHandler>();
        return builder;
    }

    public static WebApplication MapFeatures(this WebApplication app)
    {
        app.MapAuthGroup();
        app.MapUsersGroup();
        app.MapRecipesGroup();
        app.MapTagsGroup();
        return app;
    }
}
