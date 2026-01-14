using ErrorOr;
using MediatR;

namespace NutriTrack.Notifications.Application.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand(
    Guid UserId
) : IRequest<ErrorOr<Unit>>;
