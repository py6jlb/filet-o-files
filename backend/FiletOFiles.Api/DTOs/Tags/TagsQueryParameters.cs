using System;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.DTOs.Tags;

public sealed record TagsQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }

    public string? Sort { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
