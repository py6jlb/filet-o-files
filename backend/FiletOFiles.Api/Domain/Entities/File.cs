using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public class File : Entity
{
    public required string FileName { get; set; }
    public string? MimeType { get; set; }
    public required string Source { get; set; }
    public long Size { get; set; }
    public bool IsTitle { get; set; }

    public long? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
}
