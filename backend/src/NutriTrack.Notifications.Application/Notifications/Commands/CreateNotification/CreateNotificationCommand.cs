using MediatR;
using NutriTrack.Notifications.Domain.Notifications;

namespace NutriTrack.Notifications.Application.Notifications.Commands.CreateNotification;

public sealed record CreateNotificationCommand(
    Guid UserId,
    string Title,
    string Message,
    NotificationType Type,
    DateTime OccurredAtUtc,
    string? LinkUrl,
    string? MetadataJson
) : IRequest;
