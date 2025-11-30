using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Common;
using NutriTrack.Infrastructure.Persistence.Configurations;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.HasKey(r => r.Id);
        b.Property(r => r.Id)
            .HasRoleIdConversion();

        b.Property(r => r.Name)
            .HasMaxLength(DomainConstraints.Authorization.MaxRoleNameLength)
            .IsRequired();

        b.Property(r => r.Description)
            .HasMaxLength(DomainConstraints.Authorization.MaxRoleDescriptionLength);

        b.Property(r => r.IsSystemRole)
            .IsRequired();

        b.Property(r => r.IsActive)
            .IsRequired();

        b.HasIndex(r => r.Name).IsUnique();
        b.HasIndex(r => r.IsActive);

        b.ConfigureAuditable();
    }
}
