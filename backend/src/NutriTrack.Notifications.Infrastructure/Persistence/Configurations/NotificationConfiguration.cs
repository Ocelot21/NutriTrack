using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Notifications.Domain.Notifications;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Infrastructure.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new NotificationId(value));

        builder.Property(n => n.UserId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.OccurredAtUtc)
            .IsRequired();

        builder.Property(n => n.ReadAtUtc);

        builder.Property(n => n.LinkUrl)
            .HasMaxLength(500);

        builder.Property(n => n.MetadataJson);

        builder.Property(n => n.CreatedAtUtc)
            .IsRequired();

        builder.Property(n => n.ModifiedAtUtc);

        builder.Property(n => n.CreatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : (UserId?)null);

        builder.Property(n => n.ModifiedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new UserId(value.Value) : (UserId?)null);

        builder.HasIndex(n => new { n.UserId, n.Status, n.CreatedAtUtc });
    }
}
