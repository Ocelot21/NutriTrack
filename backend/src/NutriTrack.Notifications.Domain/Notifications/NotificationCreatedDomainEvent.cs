using System;
using NutriTrack.Notifications.Domain.Common.Events;
using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Domain.Notifications;

public sealed record NotificationCreatedDomainEvent(
    NotificationId NotificationId,
    UserId UserId
) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
