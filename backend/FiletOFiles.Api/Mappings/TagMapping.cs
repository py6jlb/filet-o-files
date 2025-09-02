using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Models;

namespace FiletOFiles.Api.Mappings;

internal static class TagMapping
{
    public static Tag ToTag(this AddTagRequest request)
    {
        return new() { Color = request.Color, Name = request.Name };
    }

    public static GetTagResponse ToResponse(this Tag tag)
    {
        return new(tag.Id, tag.Name, tag.Color);
    }
}
