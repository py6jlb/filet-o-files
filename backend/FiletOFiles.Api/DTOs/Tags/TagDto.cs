using System;

namespace FiletOFiles.Api.DTOs.Tags;

public sealed record TagDto
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Color { get; init; }
}
