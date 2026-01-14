using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class GoalProgressSnapshotConfiguration : IEntityTypeConfiguration<GoalProgressSnapshot>
{
    public void Configure(EntityTypeBuilder<GoalProgressSnapshot> builder)
    {
        builder.ToTable("GoalProgressSnapshots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasGoalProgressSnapshotIdConversion();

        builder.Property(x => x.UserGoalId)
            .HasUserGoalIdConversion()
            .IsRequired();

        builder.HasOne(x => x.UserGoal)
            .WithMany()
            .HasForeignKey(x => x.UserGoalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.GoalType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.GoalStartDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .IsRequired();

        builder.Property(x => x.GoalTargetDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .IsRequired();

        builder.Property(x => x.SnapshotDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .IsRequired();

        builder.Property(x => x.StartWeightKg)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.TargetWeightKg)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.OwnsMany(x => x.Points, points =>
        {
            points.ToTable("GoalProgressSnapshotPoints");

            points.WithOwner().HasForeignKey("GoalProgressSnapshotId");

            points.Property<int>("Id");
            points.HasKey("Id");

            points.Property(p => p.Date)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v))
                .IsRequired();

            points.Property(p => p.WeightKg)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            points.HasIndex("GoalProgressSnapshotId", nameof(GoalProgressSnapshotPoint.Date));
        });

        builder.Navigation(x => x.Points).Metadata.SetField("_points");
        builder.Navigation(x => x.Points).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
