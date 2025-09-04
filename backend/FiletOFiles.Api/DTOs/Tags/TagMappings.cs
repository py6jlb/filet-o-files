using System;
using FiletOFiles.Api.Domain.Entities;

namespace FiletOFiles.Api.DTOs.Tags;

public static class TagMappings
{
    public static Tag ToEntity(this CreateTagDto dto)
    {
        return new()
        {
            Id = $"t_{Ulid.NewUlid()}",
            Color = dto.Color,
            Name = dto.Name,
        };
    }

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Color = tag.Color,
            Name = tag.Name,
        };
    }
}
