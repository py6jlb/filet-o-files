using FiletOFiles.Api.Features.Auth.LoginUser;
using FiletOFiles.Api.Features.Auth.RegisterUser;
using FiletOFiles.Api.Features.Auth.TokenRefresh;
using FiletOFiles.Api.Features.Recipes.AddRecipe;
using FiletOFiles.Api.Features.Recipes.DeleteRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipe;
using FiletOFiles.Api.Features.Recipes.GetRecipes;
using FiletOFiles.Api.Features.Recipes.UpdateRecipe;
using FiletOFiles.Api.Features.RecipeTags.RemoveRecipeTag;
using FiletOFiles.Api.Features.RecipeTags.UpsertRecipeTags;
using FiletOFiles.Api.Features.Tags.AddTag;
using FiletOFiles.Api.Features.Tags.DeleteTag;
using FiletOFiles.Api.Features.Tags.GetTag;
using FiletOFiles.Api.Features.Tags.GetTags;
using FiletOFiles.Api.Features.Tags.UpdateTag;
using FiletOFiles.Api.Features.Users.GetUser;

namespace FiletOFiles.Api.Extensions;

public static class FeatureStartupExtensions
{
    public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IGetRecipeHandler, GetRecipeHandler>();
        builder.Services.AddScoped<IGetRecipesHandler, GetRecipesHandler>();
        builder.Services.AddScoped<IAddRecipeHandler, AddRecipeHandler>();
        builder.Services.AddScoped<IUpdateRecipeHandler, UpdateRecipeHandler>();
        builder.Services.AddScoped<IDeleteRecipeHandler, DeleteRecipeHandler>();

        builder.Services.AddScoped<IAddTagHandler, AddTagHandler>();
        builder.Services.AddScoped<IGetTagHandler, GetTagHandler>();
        builder.Services.AddScoped<IGetTagsHandler, GetTagsHandler>();
        builder.Services.AddScoped<IDeleteTagHandler, DeleteTagHandler>();
        builder.Services.AddScoped<IUpdateTagHandler, UpdateTagHandler>();

        builder.Services.AddScoped<IRemoveRecipeTagHandler, RemoveRecipeTagHandler>();
        builder.Services.AddScoped<IUpsertRecipeTagsHandler, UpsertRecipeTagsHandler>();

        builder.Services.AddScoped<IGetUserHandler, GetUserHandler>();

        builder.Services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
        builder.Services.AddScoped<ILoginUserHandler, LoginUserHandler>();
        builder.Services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
        return builder;
    }
}
