using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public sealed class RecipeConfigurations : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(h => h.Id);

        builder
            .HasMany(c => c.Tags)
            .WithMany(s => s.Recipes)
            .UsingEntity(j => j.ToTable("RecipeTag"));

        builder
            .HasMany(r => r.Files)
            .WithOne(f => f.Recipe)
            .HasForeignKey(x => x.RecipeId)
            .IsRequired(false);

        builder.HasIndex(r => r.Title);
    }
}
