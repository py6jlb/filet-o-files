using System;

namespace FiletOFiles.Api.Models;

public class AddRecipeRequest
{
    public string Title { get; set; }
    public string? Descriptions { get; set; }
}
