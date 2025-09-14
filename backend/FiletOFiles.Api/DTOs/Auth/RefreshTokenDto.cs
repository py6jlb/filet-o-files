using System;

namespace FiletOFiles.Api.DTOs.Auth;

public sealed record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}
