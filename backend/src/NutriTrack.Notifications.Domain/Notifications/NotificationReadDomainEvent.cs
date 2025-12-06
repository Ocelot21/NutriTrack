using System;
using NutriTrack.Notifications.Domain.Common.Events;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Domain.Notifications;

public sealed record NotificationReadDomainEvent(
    NotificationId NotificationId,
    UserId UserId,
    DateTime ReadAtUtc
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
