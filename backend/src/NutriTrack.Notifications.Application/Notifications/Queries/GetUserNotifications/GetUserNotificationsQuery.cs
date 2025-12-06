using MediatR;
using NutriTrack.Notifications.Application.Notifications.Common;
using NutriTrack.Notifications.Domain.Notifications;

namespace NutriTrack.Notifications.Application.Notifications.Queries.GetUserNotifications;

public sealed record GetUserNotificationsQuery(
    Guid UserId,
    NotificationStatus? Status,
    int? Take
) : IRequest<IReadOnlyList<NotificationResult>>;