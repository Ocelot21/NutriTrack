using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.UserGoals;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public sealed class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(EntityTypeBuilder<UserGoal> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasUserGoalIdConversion();

        b.Property(x => x.UserId).HasUserIdConversion().IsRequired();
        
        b.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.StartDate).IsRequired();
        b.Property(x => x.TargetDate).IsRequired();

        b.Property(x => x.StartWeightKg).HasPrecision(6, 2).IsRequired();
        b.Property(x => x.TargetWeightKg).HasPrecision(6, 2).IsRequired();

        b.Property(x => x.CompletedAtUtc);
        b.Property(x => x.FailedAtUtc);

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.Status });

        b.ConfigureAuditable();
    }
}
