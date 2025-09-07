using System;
using CSharpFunctionalExtensions;

namespace FiletOFiles.Api.Domain.Entities;

public sealed class User : Entity<string>
{
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public string IdentityId { get; set; }
}
