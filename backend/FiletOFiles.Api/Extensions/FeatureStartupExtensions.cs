using FiletOFiles.Api.Features.Auth;
using FiletOFiles.Api.Features.Recipes;
using FiletOFiles.Api.Features.Tags;
using FiletOFiles.Api.Features.Users;

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
    }
}
