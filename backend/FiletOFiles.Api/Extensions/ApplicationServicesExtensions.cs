using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.Services.Sorting;
using FluentValidation;

namespace FiletOFiles.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.AddSingleton<
            ISortMappingDefinition,
            SortMappingDefinition<RecipeDto, Recipe>
        >(_ => RecipeMapping.SortMapping);

        //builder.Services.AddTransient<DataShapingService>();

        builder.Services.AddHttpContextAccessor();
        //builder.Services.AddTransient<LinkService>();

        return builder;
    }
}
