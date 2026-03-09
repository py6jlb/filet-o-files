using System;

namespace FiletOFiles.Api.DTOs.RecipeTag;

public sealed record UpsertRecipeTagsDto
{
    public List<RecipeTagDto> Tags { get; set; }
}
