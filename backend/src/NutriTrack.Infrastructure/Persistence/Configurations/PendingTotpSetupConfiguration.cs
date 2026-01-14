using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Infrastructure.Persistence.TwoFactor;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class PendingTotpSetupConfiguration : IEntityTypeConfiguration<PendingTotpSetupEntity>
{
    public void Configure(EntityTypeBuilder<PendingTotpSetupEntity> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId)
            .HasUserIdConversion();

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.SecretProtected).HasMaxLength(512);

        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}