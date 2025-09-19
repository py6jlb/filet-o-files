using System;
using System.Text;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.DTOs.Tags;
using FiletOFiles.Api.Infrastructure.Database;
using FiletOFiles.Api.Services;
using FiletOFiles.Api.Services.Sorting;
using FiletOFiles.Api.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FiletOFiles.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddOpenApi();
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.AddTransient<TokenProvider>();

        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagDto, Tag>>(
            _ => TagMappings.SortMapping
        );
        builder.Services.AddHttpContextAccessor();

        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(
        this WebApplicationBuilder builder
    )
    {
        builder
            .Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbIdentityContext>();

        builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

        var oidc = builder.Configuration.GetSection("Oidc").Get<OpenIdOptions>()!;

        builder
            .Services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<OpenIdDbContext>();
            })
            .AddServer(opt =>
            {
                opt.SetAuthorizationEndpointUris("auth/authorize")
                    .SetIntrospectionEndpointUris("auth/introspect")
                    .SetTokenEndpointUris("auth/token");

                opt.AllowAuthorizationCodeFlow()
                    .AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow();

                opt.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(oidc.Key)));

                opt.AddDevelopmentSigningCertificate();

                var aspOpt = opt.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough();

                if (builder.Environment.IsDevelopment())
                {
                    aspOpt.DisableTransportSecurityRequirement();
                }
            })
            .AddValidation(opt =>
            {
                opt.UseLocalServer();
                opt.UseAspNetCore();
            });

        builder
            .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        builder.Services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(oidc.RedirectUri)
            )
        );

        builder.Services.AddAuthorization();
        return builder;
    }
}
