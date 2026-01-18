using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Auth;
using FiletOFiles.Api.DTOs.AuthManagement;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Features.Files;
using FiletOFiles.Api.Helpers;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Services.Sorting;
using FiletOFiles.Api.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FiletOFiles.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Auth"));
        builder.Services.AddTransient<TokenProvider>();
        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagDto, Tag>>(
            _ => TagMappings.SortMapping
        );
        var filestorage = builder.Configuration.GetSection("Filestorage").Get<Filestorage>()!;
        Directory.CreateDirectory(filestorage.Path);

        builder.Services.Configure<Filestorage>(builder.Configuration.GetSection("Filestorage"));
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<FilesService>();
        builder.Services.AddHttpContextAccessor();

        var persistence = builder.Configuration.GetSection("Persistence").Get<Persistence>()!;
        builder
            .Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(persistence.Path));

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(
        this WebApplicationBuilder builder
    )
    {
        builder
            .Services.AddIdentity<AppIdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbIdentityContext>()
            .AddDefaultTokenProviders();

        var authOpt = builder.Configuration.GetSection("Auth").Get<AuthOptions>()!;

        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authOpt.Issuer,
                    ValidAudience = authOpt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authOpt.Key)
                    ),
                    NameClaimType = JwtRegisteredClaimNames.Email,
                    RoleClaimType = JwtCustomClaimNames.Role,
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            );
        });
        return builder;
    }
}
