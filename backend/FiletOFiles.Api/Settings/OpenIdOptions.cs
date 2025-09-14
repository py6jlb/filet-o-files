using System;

namespace FiletOFiles.Api.Settings;

public sealed record OpenIdOptions
{
    public string RedirectUri { get; init; }
    public string Application { get; init; }
    public string Key { get; init; }
}
