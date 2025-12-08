using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Achievements;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasAchievementIdConversion();

        b.Property(x => x.Key).IsRequired().HasMaxLength(100);
        b.Property(x => x.Title).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        b.Property(x => x.Points).IsRequired();
        b.Property(x => x.Category).IsRequired();
        b.Property(x => x.IconName).HasMaxLength(100);

        b.HasIndex(x => x.Title).IsUnique();

        b.ConfigureAuditable();
    }
}
