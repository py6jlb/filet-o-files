using System;
using FiletOFiles.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiletOFiles.Api.Infrastructure.Database.Configurations;

public class RegistrationRequestConfigurations : IEntityTypeConfiguration<RegistrationRequest>
{
    public void Configure(EntityTypeBuilder<RegistrationRequest> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(u => u.Id).HasMaxLength(500);
        builder.Property(u => u.TelegramId).HasMaxLength(500).IsRequired();
        builder.Property(u => u.TelegramUserName).HasMaxLength(500);
    }
}
