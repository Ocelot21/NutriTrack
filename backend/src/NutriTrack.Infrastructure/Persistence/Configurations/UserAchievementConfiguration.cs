using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Achievements;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasUserAchievementIdConversion();

        b.Property(x => x.UserId).HasUserIdConversion().IsRequired();
        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.AchievementId).HasAchievementIdConversion().IsRequired();
        b.HasOne(x => x.Achievement)
            .WithMany()
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();

        b.ConfigureAuditable();
    }
}
