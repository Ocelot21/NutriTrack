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
}
