using System;
using Microsoft.AspNetCore.Identity;

namespace FiletOFiles.Api.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }

    public AppIdentityUser User { get; set; }
}
