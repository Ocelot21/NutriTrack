using MediatR;

namespace NutriTrack.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    Guid NotificationId
) : IRequest;
