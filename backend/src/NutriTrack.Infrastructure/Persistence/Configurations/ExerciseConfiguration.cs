using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Id)
            .HasExerciseIdConversion();

        b.Property(e => e.Name)
            .HasMaxLength(DomainConstraints.Exercises.MaxNameLength)
            .IsRequired();

        b.Property(e => e.Description)
            .HasMaxLength(DomainConstraints.Exercises.MaxDescriptionLength);

        b.Property(e => e.Category)
            .IsRequired();

        b.Property(e => e.DefaultCaloriesPerMinute)
            .HasPrecision(7, 2)
            .IsRequired();

        b.Property(e => e.ImageUrl)
            .HasMaxLength(512);

        b.Property(e => e.IsApproved)
            .IsRequired();

        b.Property(e => e.IsDeleted)
            .IsRequired();

        b.HasIndex(e => e.Name);
        b.HasIndex(e => e.Category);
        b.HasIndex(e => e.IsApproved);

        b.ConfigureAuditable();
    }
}
