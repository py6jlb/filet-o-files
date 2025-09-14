using System;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Infrastructure.Database;

public class OpenIdDbContext : DbContext
{
    public OpenIdDbContext(DbContextOptions<OpenIdDbContext> options)
        : base(options) { }
}
