using FiletOFiles.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiletOFiles.Api.Infrastructure;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
    }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Recipe>()
                .HasMany(c => c.Tags)
                .WithMany(s => s.Recipes)
                .UsingEntity(j => j.ToTable("RecipeTag"));

        mb.Entity<Recipe>().HasIndex(r => r.Title);
        mb.Entity<Tag>().HasIndex(t => t.Name);

        mb.Entity<Recipe>()
            .HasMany(r => r.Files)
            .WithOne(f => f.Recipe)
            .HasForeignKey(x => x.RecipeId)
            .IsRequired(false);
    }

}
