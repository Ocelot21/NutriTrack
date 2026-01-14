using ErrorOr;
using MediatR;
using NutriTrack.Notifications.Application.Common.Models;
using NutriTrack.Notifications.Application.Notifications.Common;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUserNotifications;

public sealed record GetUserNotificationsQuery(
    Guid UserId,
    int Page,
    int PageSize,
    bool OnlyUnread
) : IRequest<ErrorOr<PagedResult<NotificationResult>>>;