using System;

namespace FiletOFiles.Api.Models;

public class GetFileResponse
{
    public long Id { get; set; }
    public string FileName { get; set; }
    public string? MimeType { get; set; }
    public string Source { get; set; }
    public long Size { get; set; }
    public bool IsTitle { get; set; }
}
