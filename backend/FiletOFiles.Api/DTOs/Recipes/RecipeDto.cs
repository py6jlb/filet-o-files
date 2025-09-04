using System;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.DTOs.Recipes;

public sealed record RecipesCollectionDto(IEnumerable<RecipeDto> Data);

public sealed record RecipeDto
{
    public string Id { get; init; }
    public DateTime Created { get; init; }
    public string Title { get; init; }
    public string? Descriptions { get; init; }
    public IEnumerable<TagDto> Tags { get; init; }
    public IEnumerable<FileDto> Files { get; init; }
}
