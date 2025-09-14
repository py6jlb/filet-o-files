using System;
using FiletOFiles.Api.Infrastructure.Database;
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
