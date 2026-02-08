using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class UserExerciseLogConfiguration : IEntityTypeConfiguration<UserExerciseLog>
{
    public void Configure(EntityTypeBuilder<UserExerciseLog> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
            .HasUserExerciseLogIdConversion();

        b.Property(x => x.UserId)
            .HasUserIdConversion()
            .IsRequired();

        b.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.ExerciseId)
            .HasExerciseIdConversion()
            .IsRequired();

        b.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        b.OwnsOne(x => x.Snapshot, nb =>
        {
            nb.Property(s => s.ExerciseName)
                .HasMaxLength(DomainConstraints.Exercises.MaxNameLength)
                .HasColumnName("ExerciseName")
                .IsRequired();

            nb.Property(s => s.Category)
                .HasColumnName("ExerciseCategory")
                .IsRequired();

            nb.Property(s => s.CaloriesPerMinute)
                .HasPrecision(7, 2)
                .HasColumnName("CaloriesPerMinute")
                .IsRequired();

            nb.WithOwner();
        });

        b.Property(x => x.DurationMinutes)
            .HasPrecision(7, 2)
            .IsRequired();

        b.Property(x => x.OccurredAtUtc)
            .IsRequired();

        b.Property(x => x.OccurredAtLocal)
            .HasColumnType("datetimeoffset")
            .IsRequired();
        b.Property(x => x.TotalCalories)
            .HasPrecision(7, 2);

        b.Property(x => x.LocalDate)
            .IsRequired();

        b.Property(x => x.Notes)
            .HasMaxLength(DomainConstraints.UserExerciseLogs.MaxNotesLength);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExerciseId);
        b.HasIndex(x => x.LocalDate);
        b.HasIndex(x => x.OccurredAtUtc);

        b.ConfigureAuditable();
    }
}
