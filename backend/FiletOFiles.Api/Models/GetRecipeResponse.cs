using System;

namespace FiletOFiles.Api.Models;

public class GetRecipeResponse
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string? Descriptions { get; set; }
    public IEnumerable<GetTagResponse> Tags { get; set; } = [];
    public IEnumerable<GetFileResponse> Files { get; set; } = [];
}
