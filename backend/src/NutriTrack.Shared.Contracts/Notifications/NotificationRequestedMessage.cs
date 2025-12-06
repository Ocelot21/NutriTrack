namespace NutriTrack.Shared.Contracts.Notifications;

public sealed record NotificationRequestedMessage(
    Guid UserId,
    string Title,
    string Message,
    string Type,
    DateTime OccurredAtUtc,
    string? LinkUrl,
    string? MetadataJson);
