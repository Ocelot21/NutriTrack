using ErrorOr;
using MediatR;
using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;
using NutriTrack.Notifications.Application.Common.Mappings;
using NutriTrack.Notifications.Application.Common.Models;
using NutriTrack.Notifications.Application.Notifications.Common;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUserNotifications;

public sealed class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, ErrorOr<PagedResult<NotificationResult>>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<ErrorOr<PagedResult<NotificationResult>>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);


        var notifications = await _notificationRepository.GetForCurrentUserAsync(
            userId, request.Page, request.PageSize, request.OnlyUnread, cancellationToken);

        return notifications.ToPagedNotificationResult();
    }
}