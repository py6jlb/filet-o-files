using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.Features.Auth;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var identityDb =
            scope.ServiceProvider.GetRequiredService<AppDbIdentityContext>();
        try
        {
            await db.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных приложения применены успешно.");
            await identityDb.Database.MigrateAsync();
            app.Logger.LogInformation("Миграции базы данных identity применены успешно.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "Ошибка применения миграций базы данных.");
            throw;
        }
    }

    public static async Task SeedOpenidData(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(Roles.Member))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Member));
        }
        app.Logger.LogInformation("Roles created successfully");

        var oidc = app.Configuration.GetSection("Auth").Get<AuthOptions>()!;

        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        var request = new RegisterUserDto()
        {
            Email = oidc.AdminEmail,
            Name = "admin",
            Password = oidc.AdminPassword,
            ConfirmationPassword = oidc.AdminPassword,
        };

        var res = await authService.Register(request, true);
        if (res.IsFailed)
        {
            var encoderSettings = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(
                    UnicodeRanges.BasicLatin,
                    UnicodeRanges.Cyrillic
                ),
            };

            foreach (var error in res.Errors)
            {
                string jsonString = JsonSerializer.Serialize(error, encoderSettings);
                app.Logger.LogInformation("Пользователь не создан: {ErrorMessage}", jsonString);
            }
        }
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

        return builder;
    }
}
