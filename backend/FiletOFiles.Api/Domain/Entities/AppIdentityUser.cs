using System;
using Microsoft.AspNetCore.Identity;

namespace FiletOFiles.Api.Domain.Entities;

public sealed class AppIdentityUser : IdentityUser
{
    public bool IsApproved { get; set; }
    public bool MustChangePassword { get; set; }
}
