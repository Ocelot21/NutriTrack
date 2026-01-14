using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class GroceryConfiguration : IEntityTypeConfiguration<Grocery>
{
    public void Configure(EntityTypeBuilder<Grocery> b)
    {
        b.HasKey(g => g.Id);
        b.Property(g => g.Id)
            .HasGroceryIdConversion();

        b.Property(g => g.Name)
            .HasMaxLength(DomainConstraints.Groceries.MaxNameLength)
            .IsRequired();

        b.Property(g => g.Category)
            .IsRequired();

        b.Property(g => g.Barcode)
            .HasMaxLength(DomainConstraints.Groceries.MaxBarcodeLength);

        b.OwnsOne(g => g.MacrosPer100, nb =>
        {
            nb.Property(m => m.ProteinGramsPer100)
                .HasPrecision(5, 2)
                .HasColumnName("ProteinGramsPer100g")
                .IsRequired();

            nb.Property(m => m.CarbsGramsPer100)
                .HasPrecision(5, 2)
                .HasColumnName("CarbsGramsPer100g")
                .IsRequired();

            nb.Property(m => m.FatGramsPer100)
                .HasPrecision(5, 2)
                .HasColumnName("FatGramsPer100g")
                .IsRequired();
        });

        b.Property(g => g.CaloriesPer100)
            .HasColumnType("smallint")
            .IsRequired();

        b.Property(g => g.UnitOfMeasure)
            .IsRequired();

        b.Property(g => g.ImageUrl)
            .HasMaxLength(512);

        b.Property(g => g.IsApproved)
            .IsRequired();

        b.Property(g => g.IsDeleted)
            .IsRequired();

        b.HasIndex(g => g.Name);
        b.HasIndex(g => g.Barcode).IsUnique(false);
        b.HasIndex(g => g.Category);

        b.ConfigureAuditable();
    }
}