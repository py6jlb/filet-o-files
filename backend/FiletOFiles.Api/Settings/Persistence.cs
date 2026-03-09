using System;

namespace FiletOFiles.Api.Settings;

public sealed record Persistence
{
    public string Path { get; init; }
}
