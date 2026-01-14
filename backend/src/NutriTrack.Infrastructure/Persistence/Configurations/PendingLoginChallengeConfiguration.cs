using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Infrastructure.Persistence.TwoFactor;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class PendingLoginChallengeConfiguration
: IEntityTypeConfiguration<PendingLoginChallengeEntity>
{
    public void Configure(EntityTypeBuilder<PendingLoginChallengeEntity> builder)
    {
        builder.ToTable("PendingLoginChallenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.Consumed)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasUserIdConversion();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.Consumed, x.ExpiresAtUtc });
        builder.HasIndex(x => x.ExpiresAtUtc);
    }
}
