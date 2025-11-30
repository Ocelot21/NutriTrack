using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Id)
            .HasPermissionIdConversion();

        b.Property(p => p.Key)
            .HasConversion(v => v.Value, raw => PermissionKey.Create(raw))
            .HasMaxLength(DomainConstraints.Authorization.MaxPermissionKeyLength)
            .IsRequired();

        b.Property(p => p.Description)
            .HasMaxLength(DomainConstraints.Authorization.MaxPermissionDescriptionLength);

        b.HasIndex(p => p.Key).IsUnique();

        b.ConfigureAuditable();
    }
}