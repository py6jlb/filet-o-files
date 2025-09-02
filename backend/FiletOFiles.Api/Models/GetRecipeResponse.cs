using System;

namespace FiletOFiles.Api.Models;

public record GetRecipeResponse(long Id, string Title, string? Descriptions)
{
    public IEnumerable<GetTagResponse> Tags { get; set; } = [];
    public IEnumerable<GetFileResponse> Files { get; set; } = [];
}
