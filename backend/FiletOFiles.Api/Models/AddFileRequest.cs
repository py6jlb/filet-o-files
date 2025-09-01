using System;

namespace FiletOFiles.Api.Models;

public class AddFileRequest
{
    public string FileName { get; set; }
    public string? MimeType { get; set; }
    public string Source { get; set; }
    public long Size { get; set; }
    public bool IsTitle { get; set; }
    public long RecipeId { get; set; }
}
