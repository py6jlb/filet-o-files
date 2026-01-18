using System;

namespace FiletOFiles.Api.DTOs.RecipeTag;

public sealed record RecipeTagDto
{
    public string TagId { get; set; }
    public string AdditionalData { get; set; }
}
