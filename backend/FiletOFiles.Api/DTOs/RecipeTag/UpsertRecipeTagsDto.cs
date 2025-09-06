using System;

namespace FiletOFiles.Api.DTOs.RecipeTag;

public sealed record UpsertRecipeTagsDto
{
    public List<string> TagIds { get; set; }
}
