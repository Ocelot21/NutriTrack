using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Countries;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasUserIdConversion();

        builder.Property(u => u.FirstName)
            .HasMaxLength(DomainConstraints.Users.MaxNameLength)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(DomainConstraints.Users.MaxNameLength)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasConversion(v => v.Value, raw => Username.Create(raw))
            .HasMaxLength(DomainConstraints.Users.MaxUsernameLength)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(v => v.Value, raw => Email.Create(raw))
            .HasMaxLength(DomainConstraints.Users.MaxEmailLength)
            .IsRequired();

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(512);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.TimeZoneId)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(u => u.CountryCode)
            .HasOptionalCountryCodeConversion()
            .HasMaxLength(2);

        builder.HasOne(u => u.Country)
            .WithMany()
            .HasForeignKey(u => u.CountryCode)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(u => u.IsEmailVerified)
            .IsRequired();

        builder.Property(u => u.RoleId)
            .HasRoleIdConversion()
            .IsRequired();

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.TotpSecretProtected)
            .HasMaxLength(512);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.RoleId);

        builder.ConfigureAuditable();
    }
}