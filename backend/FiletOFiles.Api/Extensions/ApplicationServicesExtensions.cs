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
        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        builder.Services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagDto, Tag>>(
            _ => TagMappings.SortMapping
        );
        var filestorage = builder.Configuration.GetSection("Filestorage").Get<Filestorage>()!;
        builder.Services.AddSingleton(filestorage);
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

        var oidc = builder.Configuration.GetSection("Oidc").Get<OpenIdOptions>()!;

        builder
            .Services.AddOpenIddict()
            .AddCore(opt =>
            {
                opt.UseEntityFrameworkCore().UseDbContext<OpenIdDbContext>();
            })
            .AddServer(opt =>
            {
                opt.SetTokenEndpointUris("auth/token")
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow()
                    .AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(oidc.Key)));

                var aspOpt = opt.UseAspNetCore().EnableTokenEndpointPassthrough();

                if (builder.Environment.IsDevelopment())
                {
                    opt.AddDevelopmentSigningCertificate();
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
