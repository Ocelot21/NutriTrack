namespace NutriTrack.Application.Notifications.Messages;

public sealed record NotificationRequestedMessage(
    Guid UserId,
    string Title,
    string Message,
    string Type,
    DateTime OccurredAtUtc,
    string? LinkUrl,
    string? MetadataJson);
