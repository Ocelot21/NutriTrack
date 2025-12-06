using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Notifications.Common;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUserNotifications;

public sealed class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, IReadOnlyList<NotificationResult>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IReadOnlyList<NotificationResult>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var notifications = await _notificationRepository.GetForUserAsync(
            userId,
            request.Status,
            request.Take,
            cancellationToken);

        return notifications
            .Select(n => new NotificationResult(
                n.Id.Value,
                n.Title,
                n.Message,
                n.Type,
                n.Status,
                n.OccurredAtUtc,
                n.ReadAtUtc,
                n.LinkUrl))
            .ToList();
    }
}
