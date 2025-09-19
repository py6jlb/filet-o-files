using System;

namespace FiletOFiles.Api.Settings;

public sealed record Filestorage
{
    public string Path { get; init; }
}
