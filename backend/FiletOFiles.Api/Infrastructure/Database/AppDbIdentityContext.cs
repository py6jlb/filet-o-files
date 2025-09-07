using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Infrastructure.Database;

public sealed class AppDbIdentityContext : IdentityDbContext
{
    public AppDbIdentityContext(DbContextOptions<AppDbIdentityContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
