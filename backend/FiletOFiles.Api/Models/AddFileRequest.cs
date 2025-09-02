using System;

namespace FiletOFiles.Api.Models;

public record AddFileRequest(
    string FileName,
    string MimeType,
    string Source,
    long Size,
    bool IsTitle,
    long RecipeId
);
