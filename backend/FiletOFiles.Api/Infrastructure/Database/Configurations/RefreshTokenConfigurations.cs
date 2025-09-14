using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).HasMaxLength(300);
        builder.Property(r => r.Token).HasMaxLength(1000);
        builder.HasIndex(r => r.Token).IsUnique();

        builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
