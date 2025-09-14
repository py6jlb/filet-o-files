using System;
using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Services.Sorting;

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

    public static void UpdateFromDto(this Tag tag, UpdateTagDto dto)
    {
        tag.Name = dto.Name;
        tag.Color = dto.Color;
    }

    public static readonly SortMappingDefinition<TagDto, Tag> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(TagDto.Name), nameof(Tag.Name)),
            new SortMapping(nameof(TagDto.Color), nameof(Tag.Color)),
        ],
    };
}
