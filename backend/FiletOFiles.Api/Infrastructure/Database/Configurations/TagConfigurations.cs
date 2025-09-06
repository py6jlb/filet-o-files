using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public sealed class TagConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Color).IsRequired();
        builder.Property(t => t.Name).IsRequired();
        builder.HasIndex(t => new { t.Name }).IsUnique();

        builder.HasMany(h => h.Recipes).WithMany().UsingEntity<RecipeTag>();
    }
}
