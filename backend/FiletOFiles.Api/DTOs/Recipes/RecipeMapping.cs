using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.DTOs.Recipes;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.DTOs.Recipes;

internal static class RecipeMapping
{
    public static Recipe ToEntity(this RecipeDto dto)
    {
        return new Recipe
        {
            Created = DateTime.Now,
            Descriptions = dto.Descriptions,
            Title = dto.Title,
            Id = $"r_{Ulid.NewUlid()}",
        };
    }

    public static RecipeDto ToDto(this Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Descriptions = recipe.Descriptions,
            Created = recipe.Created,
            Tags = recipe.Tags.Select(x => x.ToDto()) ?? [],
            Files = recipe.Files.Select(x => x.ToDto()) ?? [],
        };
    }
}
