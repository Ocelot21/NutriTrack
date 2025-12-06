using Microsoft.EntityFrameworkCore;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Domain.Notifications;
using NutriTrack.Notifications.Domain.Users;
using NutriTrack.Notifications.Infrastructure.Persistence;

namespace NutriTrack.Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _dbContext;

    public NotificationRepository(NotificationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Notification notification)
    {
        _dbContext.Notifications.Add(notification);
    }

    public async Task<Notification?> GetByIdAsync(
        NotificationId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetForUserAsync(
        UserId userId,
        NotificationStatus? status = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Notification> query = _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAtUtc);

        if (status is not null)
        {
            query = query.Where(n => n.Status == status);
        }

        if (take is > 0)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
