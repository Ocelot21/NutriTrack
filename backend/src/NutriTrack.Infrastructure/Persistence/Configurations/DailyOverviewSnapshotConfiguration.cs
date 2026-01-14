using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class DailyOverviewSnapshotConfiguration : IEntityTypeConfiguration<DailyOverviewSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyOverviewSnapshot> builder)
    {
        builder.ToTable("DailyOverviewSnapshots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDailyOverviewSnapshotIdConversion();

        builder.Property(x => x.Date)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .IsRequired();

        builder.Property(x => x.TargetCalories).IsRequired();
        builder.Property(x => x.TargetProteinGrams).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TargetFatGrams).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TargetCarbohydrateGrams).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.ConsumedCalories).IsRequired();
        builder.Property(x => x.BurnedCalories).IsRequired();
        builder.Property(x => x.NetCalories).IsRequired();
        builder.Property(x => x.RemainingCalories).IsRequired();

        builder.Property(x => x.ConsumedProteinGrams).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.ConsumedFatGrams).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.ConsumedCarbohydrateGrams).HasColumnType("decimal(18,2)").IsRequired();

        builder.Property(x => x.MealCount).IsRequired();
        builder.Property(x => x.ExerciseCount).IsRequired();
    }
}
