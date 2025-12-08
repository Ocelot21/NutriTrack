using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Notifications;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence
{
    public interface IUserNotificationsReadRepository
    {
        Task<PagedResult<NotificationReadModel>> GetForCurrentUserAsync(
        UserId userId,
        int page,
        int pageSize,
        bool onlyUnread,
        CancellationToken cancellationToken = default);
    }
}