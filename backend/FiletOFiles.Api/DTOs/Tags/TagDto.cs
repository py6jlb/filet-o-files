using System;
using FiletOFiles.Api.DTOs.Common;

namespace FiletOFiles.Api.DTOs.Tags;

public sealed record TagsCollectionDto : ICollectionResponse<TagDto>
{
    public List<TagDto> Items { get; init; }
};

public sealed record TagDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }
}
