using System;

namespace FiletOFiles.Api.DTOs.Recipes;

public sealed record UpdateRecipeDto
{
    public string Title { get; init; }
    public string? Descriptions { get; init; }
}
