using ErrorOr;
using MediatR;

namespace NutriTrack.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    Guid UserId,
    Guid NotificationId
) : IRequest<ErrorOr<Unit>>;
