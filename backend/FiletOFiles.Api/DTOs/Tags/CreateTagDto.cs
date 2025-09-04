using System;

namespace FiletOFiles.Api.DTOs.Tags;

public sealed record CreateTagDto
{
    public string Name { get; init; }
    public string Color { get; init; }
}
