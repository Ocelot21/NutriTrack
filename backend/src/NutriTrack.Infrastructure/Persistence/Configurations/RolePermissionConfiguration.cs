using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        b.Property(rp => rp.RoleId)
            .HasRoleIdConversion()
            .IsRequired();

        b.Property(rp => rp.PermissionId)
            .HasPermissionIdConversion()
            .IsRequired();

        b.HasOne(rp => rp.Role)
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(rp => rp.PermissionId);
        b.HasIndex(rp => rp.RoleId);
    }
}
