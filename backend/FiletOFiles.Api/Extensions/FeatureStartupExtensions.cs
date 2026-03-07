using FiletOFiles.Api.Features.Auth;
using FiletOFiles.Api.Features.AuthManagement;
using FiletOFiles.Api.Features.Files;
using FiletOFiles.Api.Features.Recipes;
using FiletOFiles.Api.Features.Tags;
using FiletOFiles.Api.Features.Users;

namespace FiletOFiles.Api.Extensions;

public static class FeatureStartupExtensions
{
    public static WebApplication MapFeatures(this WebApplication app)
    {
        app.MapAuthGroup();
        app.MapAuthManagementGroup();
        app.MapUsersGroup();
        app.MapRecipesGroup();
        app.MapTagsGroup();
        app.MapFilesGroup();
        return app;
    }
}
