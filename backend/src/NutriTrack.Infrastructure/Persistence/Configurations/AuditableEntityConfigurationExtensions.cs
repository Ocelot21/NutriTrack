using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Configurations;

public static class AuditableEntityConfigurationExtensions
{
    public static void ConfigureAuditable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, IAuditableEntity
    {
        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                guid => guid.HasValue ? new UserId(guid.Value) : null);

        builder.Property(e => e.ModifiedAtUtc);

        builder.Property(e => e.ModifiedBy)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                guid => guid.HasValue ? new UserId(guid.Value) : null);
    }
}