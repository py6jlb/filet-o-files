using FiletOFiles.Api.Features.Auth;
using FiletOFiles.Api.Features.Recipes;
using FiletOFiles.Api.Features.Recipes.AddRecipe;
using FiletOFiles.Api.Features.Recipes.DeleteRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipes;
using FiletOFiles.Api.Features.Recipes.UpdateRecipe;
using FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;
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
    public static WebApplication MapFeatures(this WebApplication app)
    {
        app.MapAuthGroup();
        app.MapUsersGroup();
        app.MapRecipesGroup();
        app.MapTagsGroup();
        return app;
        builder.Services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
    }
}
