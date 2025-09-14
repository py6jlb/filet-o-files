using System;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FiletOFiles.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var identityDb =
            scope.ServiceProvider.GetRequiredService<AppDbIdentityContext>();
        await using var openIdDbContext =
            scope.ServiceProvider.GetRequiredService<OpenIdDbContext>();
        try
        {
            await db.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных приложения применены успешно.");
            await identityDb.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных identity применены успешно.");
            await openIdDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных openid применены успешно.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Ошибка применения миграций базы данных.");
            throw;
        }
    }

    public static async Task SeedOpenidData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        var oidc = app.Configuration.GetSection("Oidc").Get<OpenIdOptions>()!;
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var existApp = await manager.FindByClientIdAsync(oidc.Application);
        if (existApp != null)
        {
            return;
        }

        await manager.CreateAsync(
            new OpenIddictApplicationDescriptor
            {
                ClientId = oidc.Application,
                ClientType = ClientTypes.Public,
                RedirectUris =
                {
                    new Uri(oidc.RedirectUri),
                    new Uri($"{oidc.RedirectUri}/signin-callback.html"),
                    new Uri($"{oidc.RedirectUri}oidc.RedirectUri/signin-silent-callback.html"),
                },
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.EndSession,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.RefreshToken,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                },
                Requirements = { Requirements.Features.ProofKeyForCodeExchange },
            }
        );
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        builder.Services.AddDbContext<AppDbIdentityContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        builder.Services.AddDbContext<OpenIdDbContext>(options =>
        {
            options.UseSqlite(connectionString);
            options.UseOpenIddict();
            options.UseSnakeCaseNamingConvention();
        });

        return builder;
    }
}
