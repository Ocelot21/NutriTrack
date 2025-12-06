namespace NutriTrack.Notifications.Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}