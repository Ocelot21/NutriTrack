using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountQueryHandler
    : IRequestHandler<GetUnreadNotificationCountQuery, int>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<int> Handle(
        GetUnreadNotificationCountQuery request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        return await _notificationRepository.GetUnreadCountByUserIdAsync(userId, cancellationToken);
    }
}
