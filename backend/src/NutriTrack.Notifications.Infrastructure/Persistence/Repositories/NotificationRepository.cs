using Microsoft.EntityFrameworkCore;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Common.Models;
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

    public async Task<PagedResult<Notification>> GetForCurrentUserAsync(
        UserId userId,
        int page,
        int pageSize,
        bool onlyUnread,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        IQueryable<Notification> query = _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAtUtc);

        if (onlyUnread)
        {
            query = query.Where(n => n.Status == NotificationStatus.Unread);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Notification>(
            items,
            totalCount,
            page,
            pageSize);
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

    public async Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread)
            .CountAsync(cancellationToken);
    }
}