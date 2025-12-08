namespace NutriTrack.Contracts.Notifications;

public record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Message,
    string Type,
    string Status,
    string OccurredAtUtc,
    string ReadAtUtc,
    string? LinkUrl,
    string? MetadataJson);