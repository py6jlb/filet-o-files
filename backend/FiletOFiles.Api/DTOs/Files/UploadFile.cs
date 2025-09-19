using System;

namespace FiletOFiles.Api.DTOs.Files;

public sealed class UploadFile
{
    public string RecipeId { get; set; }
    public IFormFile File { get; set; }
    public bool IsTitle { get; set; }
}
