using NutriTrack.Notifications.Application.Common.Models;
using NutriTrack.Notifications.Domain.Notifications;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Common.Interfaces.Persistence;

public interface INotificationRepository
{
    void Add(Notification notification);

    Task<Notification?> GetByIdAsync(NotificationId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetForUserAsync(
        UserId userId,
        NotificationStatus? status = null,
        int? take = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Notification>> GetForCurrentUserAsync(
        UserId userId,
        int page,
        int pageSize,
        bool onlyUnread,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
