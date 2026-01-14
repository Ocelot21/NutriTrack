using NutriTrack.Notifications.Domain.Notifications;

namespace NutriTrack.Notifications.Application.Notifications.Common;

public sealed record NotificationResult(
    Guid Id,
    string Title,
    string Message,
    string Type,
    string Status,
    DateTime OccurredAtUtc,
    DateTime? ReadAtUtc,
    string? LinkUrl);
