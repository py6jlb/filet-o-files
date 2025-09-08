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
        var jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>()!;
        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtAuthOptions.Key)
                    ),
                };
            });

        builder.Services.AddAuthorization();
        return builder;
    }
}
