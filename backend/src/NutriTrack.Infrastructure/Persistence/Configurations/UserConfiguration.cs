using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(u => u.Id);
        b.Property(u => u.Id)
            .HasUserIdConversion();

        b.Property(u => u.FirstName)
            .HasMaxLength(DomainConstraints.Users.MaxNameLength)
            .IsRequired();

        b.Property(u => u.LastName)
            .HasMaxLength(DomainConstraints.Users.MaxNameLength)
            .IsRequired();

        b.Property(u => u.Username)
            .HasConversion(v => v.Value, raw => Username.Create(raw))
            .HasMaxLength(DomainConstraints.Users.MaxUsernameLength)
            .IsRequired();

        b.Property(u => u.Email)
            .HasConversion(v => v.Value, raw => Email.Create(raw))
            .HasMaxLength(DomainConstraints.Users.MaxEmailLength)
            .IsRequired();

        b.Property(u => u.PasswordHash)
            .IsRequired();

        b.Property(u => u.TimeZoneId)
            .HasMaxLength(128)
            .IsRequired();

        b.Property(u => u.Country)
            .HasConversion(v => v != null ? v.Value : null, raw => CountryCode.CreateOptional(raw))
            .HasMaxLength(2);

        b.Property(u => u.IsEmailVerified)
            .IsRequired();

        b.Property(u => u.RoleId)
            .HasRoleIdConversion()
            .IsRequired();

        b.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(u => u.Username).IsUnique();
        b.HasIndex(u => u.Email).IsUnique();
        b.HasIndex(u => u.RoleId);

        b.ConfigureAuditable();
    }
}
