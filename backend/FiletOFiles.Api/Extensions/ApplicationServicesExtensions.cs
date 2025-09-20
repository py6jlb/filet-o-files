using System;
using System.Text;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Features.Auth;
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
using OpenIddict.Abstractions;

namespace FiletOFiles.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagDto, Tag>>(
            _ => TagMappings.SortMapping
        );
        var filestorage = builder.Configuration.GetSection("Filestorage").Get<Filestorage>()!;
        builder.Services.AddSingleton(filestorage);
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddHttpContextAccessor();

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(
        this WebApplicationBuilder builder
    )
    {
        builder
            .Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbIdentityContext>()
            .AddDefaultTokenProviders();

        builder
            .Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/persistent"));

        var authOpt = builder.Configuration.GetSection("Auth").Get<AuthOptions>()!;

        builder
            .Services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<OpenIdDbContext>();
            })
            .AddServer(opt =>
            {
                opt.AllowAuthorizationCodeFlow().AllowRefreshTokenFlow().AllowPasswordFlow();
                opt.SetAuthorizationEndpointUris("/auth/authorize");
                opt.SetTokenEndpointUris("/auth/token");

                opt.SetAccessTokenLifetime(TimeSpan.FromMinutes(authOpt.ExpirationInMinutes))
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(authOpt.RefreshTokenExpirationDays));

                opt.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.OfflineAccess
                );

                var aspOpt = opt.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();

                if (builder.Environment.IsDevelopment())
                {
                    opt.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
                    aspOpt.DisableTransportSecurityRequirement();
                }
                else
                {
                    opt.AddEncryptionKey(
                        new SymmetricSecurityKey(Convert.FromBase64String(authOpt.Key))
                    );
                }
                opt.DisableAccessTokenEncryption();
            })
            .AddValidation(opt =>
            {
                opt.UseLocalServer();
                opt.UseAspNetCore();
            });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
        });
        builder.Services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(authOpt.RedirectUri)
            )
        );

        builder.Services.AddAuthorization();
        return builder;
    }
}
