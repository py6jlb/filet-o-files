using System;

namespace FiletOFiles.Api.Settings;

public sealed record AuthOptions
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string Key { get; init; }
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }

    public string AdminEmail { get; init; }
    public string AdminPassword { get; init; }

    public string BotToken { get; init; }
}
