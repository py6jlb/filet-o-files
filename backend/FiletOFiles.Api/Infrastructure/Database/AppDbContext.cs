using FiletOFiles.Api.Domain.Entities;
using FiletOFiles.Api.Infrastructure.Database.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using File = FiletOFiles.Api.Domain.Entities.File;

namespace FiletOFiles.Api.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RecipeTag> RecipeTag { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new TagConfigurations());
        builder.ApplyConfiguration(new FileConfigurations());
        builder.ApplyConfiguration(new RecipeConfigurations());
        builder.ApplyConfiguration(new RecipeTagConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}
