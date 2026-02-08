using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Meals;
using NutriTrack.Infrastructure.Persistence.Configurations;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> b)
    {
        b.HasKey(m => m.Id);
        b.Property(m => m.Id)
            .HasMealIdConversion();

        b.Property(m => m.UserId)
            .HasUserIdConversion()
            .IsRequired();

        b.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(m => m.Name)
            .HasMaxLength(DomainConstraints.Meals.MaxMealNameLength)
            .IsRequired();

        b.Property(m => m.Description)
            .HasMaxLength(DomainConstraints.Meals.MaxMealDescriptionLength);

        b.Property(m => m.OccurredAtUtc)
            .IsRequired();

        b.Property(m => m.OccurredAtLocal)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        b.Property(m => m.LocalDate)
            .IsRequired();

        b.Property(m => m.TotalCalories)
            .HasColumnType("int")
            .IsRequired();

        b.Property(m => m.TotalProtein)
            .HasPrecision(7, 2)
            .IsRequired();

        b.Property(m => m.TotalCarbohydrates)
            .HasPrecision(7, 2)
            .IsRequired();

        b.Property(m => m.TotalFats)
            .HasPrecision(7, 2)
            .IsRequired();

        b.OwnsMany(m => m.Items, nb =>
        {
            nb.ToTable("MealItems");

            nb.WithOwner().HasForeignKey(mi => mi.MealId);

            nb.HasKey(mi => mi.Id);
            nb.Property(mi => mi.Id)
                .HasMealItemIdConversion();

            nb.HasOne(mi => mi.Grocery)
                    .WithMany()
                    .HasForeignKey(m => m.GroceryId)
                    .OnDelete(DeleteBehavior.Restrict);

            nb.Property(mi => mi.Quantity)
                .HasPrecision(9, 2)
                .IsRequired();

            nb.OwnsOne(mi => mi.Snapshot, sb =>
            {
                sb.Property(s => s.GroceryName)
                    .HasMaxLength(DomainConstraints.Groceries.MaxNameLength)
                    .HasColumnName("GroceryName")
                    .IsRequired();

                sb.Property(s => s.CaloriesPer100)
                    .HasColumnType("smallint")
                    .HasColumnName("CaloriesPer100")
                    .IsRequired();

                sb.OwnsOne(s => s.MacrosPer100, mb =>
                {
                    mb.Property(m => m.ProteinGramsPer100)
                        .HasPrecision(5, 2)
                        .HasColumnName("ProteinGramsPer100")
                        .IsRequired();

                    mb.Property(m => m.CarbsGramsPer100)
                        .HasPrecision(5, 2)
                        .HasColumnName("CarbsGramsPer100")
                        .IsRequired();

                    mb.Property(m => m.FatGramsPer100)
                        .HasPrecision(5, 2)
                        .HasColumnName("FatGramsPer100")
                        .IsRequired();
                });

                sb.Property(s => s.UnitOfMeasure)
                    .HasColumnName("UnitOfMeasure")
                    .IsRequired();

                sb.Property(g => g.GramsPerPiece)
                    .HasColumnName("GramsPerPiece")
                    .HasPrecision(7, 2);

                sb.WithOwner();
            });

            nb.HasIndex(mi => mi.Quantity);
        });

        b.HasIndex(m => m.UserId);
        b.HasIndex(m => m.LocalDate);
        b.HasIndex(m => m.OccurredAtUtc);

        b.ConfigureAuditable();
    }
}
