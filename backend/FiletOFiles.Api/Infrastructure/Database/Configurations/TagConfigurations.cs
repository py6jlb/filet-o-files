using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public sealed class TagConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(h => h.Id);

        builder.HasIndex(t => t.Name);
    }
}
