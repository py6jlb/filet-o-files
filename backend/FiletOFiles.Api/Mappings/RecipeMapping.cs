using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Mappings;

internal static class RecipeMapping
{
    public static Recipe ToRecipe(this AddRecipeRequest request)
    {
        return new()
        {
            Created = DateTime.Now,
            Descriptions = request.Descriptions,
            Title = request.Title,
        };
    }

    public static GetRecipeResponse ToResponse(this Recipe recipe)
    {
        return new()
        {
            Descriptions = recipe.Descriptions,
            Title = recipe.Title,
            Id = recipe.Id,
            Tags = recipe.Tags.Select(x => x.ToResponse()),
            Files = recipe.Files.Select(x => x.ToResponse()),
        };
    }
}
