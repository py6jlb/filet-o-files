using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public sealed class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.HasKey(ht => new { ht.RecipeId, ht.TagId });

        builder.HasOne<Tag>().WithMany(t => t.RecipeTag).HasForeignKey(rt => rt.TagId);
        builder.HasOne<Recipe>().WithMany(r => r.RecipeTags).HasForeignKey(rt => rt.RecipeId);
    }
}
