using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Notifications;

namespace NutriTrack.Infrastructure.Notifications.Persistence;

public sealed class NotificationsReadDbContext : DbContext
{
    public NotificationsReadDbContext(DbContextOptions<NotificationsReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationReadModel> Notifications { get; set; } = null!;
}
