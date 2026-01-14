using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Social;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class SocialPostConfiguration : IEntityTypeConfiguration<SocialPost>
{
    public void Configure(EntityTypeBuilder<SocialPost> builder)
    {
        builder.ToTable(t => t
            .HasCheckConstraint("CK_SocialPosts_Type_Text",
            "[Type] <> 1 OR ([Text] IS NOT NULL AND [UserAchievementId] IS NULL)")
        );

        builder.ToTable(t => t
            .HasCheckConstraint("CK_SocialPosts_Type_Achievement",
            "[Type] <> 2 OR ([UserAchievementId] IS NOT NULL)")
        );

        builder.ToTable(t => t
            .HasCheckConstraint("CK_SocialPosts_Type_DailyOverview",
                "[Type] <> 3 OR ([DailyOverviewSnapshotId] IS NOT NULL AND [UserAchievementId] IS NULL AND [GoalProgressSnapshotId] IS NULL)")
        );

        builder.ToTable(t => t
            .HasCheckConstraint("CK_SocialPosts_Type_GoalProgress",
                "[Type] <> 4 OR ([GoalProgressSnapshotId] IS NOT NULL AND [UserAchievementId] IS NULL AND [DailyOverviewSnapshotId] IS NULL)")
        );

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasSocialPostIdConversion();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.UserId).HasUserIdConversion();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Visibility)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.Text)
            .HasMaxLength(280);

        builder.Property(x => x.UserAchievementId).HasOptionalUserAchievementIdConversion();

        builder.HasOne(x => x.UserAchievement)
            .WithMany()
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DailyOverviewSnapshotId).HasOptionalDailyOverviewSnapshotIdConversion();
        builder.HasOne(x => x.DailyOverviewSnapshot)
            .WithMany()
            .HasForeignKey(x => x.DailyOverviewSnapshotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.GoalProgressSnapshotId).HasOptionalGoalProgressSnapshotIdConversion();
        builder.HasOne(x => x.GoalProgressSnapshot)
            .WithMany()
            .HasForeignKey(x => x.GoalProgressSnapshotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

        builder.ConfigureAuditable();
    }
}
