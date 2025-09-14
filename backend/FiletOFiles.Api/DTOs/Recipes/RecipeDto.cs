using System;
using FiletOFiles.Api.DTOs.Common;
using FiletOFiles.Api.DTOs.Files;
using FiletOFiles.Api.DTOs.Tags;

namespace FiletOFiles.Api.DTOs.Recipes;

public sealed record RecipesCollectionDto : ICollectionResponse<RecipeDto>
{
    public List<RecipeDto> Items { get; init; }
}

public sealed record RecipeDto
{
    public string Id { get; init; }
    public DateTime Created { get; init; }
    public string Title { get; init; }
    public string? Descriptions { get; init; }
    public IEnumerable<TagDto> Tags { get; init; }
    public IEnumerable<FileDto> Files { get; init; }
}
