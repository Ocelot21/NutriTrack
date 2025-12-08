using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.ActivityLevelHistory;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class ActivityLevelHistoryConfiguration : IEntityTypeConfiguration<ActivityLevelHistoryEntry>
{
    public void Configure(EntityTypeBuilder<ActivityLevelHistoryEntry> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasActivityLevelHistoryIdConversion();

        b.Property(x => x.UserId).HasUserIdConversion().IsRequired();
        b.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.ActivityLevel).IsRequired();
        b.Property(x => x.EffectiveFrom).IsRequired();

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.EffectiveFrom });

        b.ConfigureAuditable();
    }
}
