using System;
using Microsoft.AspNetCore.Mvc;

namespace FiletOFiles.Api.DTOs.Recipes;

public sealed record RecipeQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public string? Fields { get; init; }
    public int Take { get; init; } = 10;
    public int Skip { get; init; } = 0;
}
