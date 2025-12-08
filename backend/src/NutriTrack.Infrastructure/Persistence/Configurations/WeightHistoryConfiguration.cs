using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class WeightHistoryConfiguration : IEntityTypeConfiguration<WeightHistoryEntry>
{
    public void Configure(EntityTypeBuilder<WeightHistoryEntry> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasWeightHistoryIdConversion();

        b.Property(x => x.UserId).HasUserIdConversion().IsRequired();
        b.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.Date).IsRequired();
        b.Property(x => x.WeightKg).HasPrecision(6, 2).IsRequired();

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.Date });

        b.ConfigureAuditable();
    }
}
