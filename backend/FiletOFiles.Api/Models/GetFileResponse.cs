using System;

namespace FiletOFiles.Api.Models;

public record GetFileResponse(
    long Id,
    string FileName,
    string MimeType,
    string Source,
    long Size,
    bool IsTitle
);
