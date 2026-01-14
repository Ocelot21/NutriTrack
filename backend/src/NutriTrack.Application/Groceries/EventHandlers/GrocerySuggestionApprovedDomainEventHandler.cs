using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Notifications;
using NutriTrack.Application.Notifications.Messages;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Groceries.Events;
using System.Text.Json;

using AppNotificationPublisher = NutriTrack.Application.Common.Interfaces.Messaging.INotificationPublisher;

namespace NutriTrack.Application.Groceries.EventHandlers;

public sealed class GrocerySuggestionApprovedDomainEventHandler
    : INotificationHandler<DomainEventNotification<GrocerySuggestionApprovedDomainEvent>>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly AppNotificationPublisher _notificationPublisher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GrocerySuggestionApprovedDomainEventHandler(
        IGroceryRepository groceryRepository,
        AppNotificationPublisher notificationPublisher,
        IDateTimeProvider dateTimeProvider)
    {
        _groceryRepository = groceryRepository;
        _notificationPublisher = notificationPublisher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        DomainEventNotification<GrocerySuggestionApprovedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var grocery = await _groceryRepository.GetByIdAsync(domainEvent.GroceryId, cancellationToken);
        if (grocery is null)
        {
            return;
        }

        var title = "Suggestion approved";
        var message = $"Your grocery suggestion '{grocery.Name}' was approved.";

        var metadata = new
        {
            suggestionType = "Grocery",
            groceryId = grocery.Id.Value,
            groceryName = grocery.Name
        };

        var notificationRequestedMessage = new NotificationRequestedMessage(
            UserId: domainEvent.SuggestedByUserId.Value,
            Title: title,
            Message: message,
            Type: "SuggestionApproved",
            OccurredAtUtc: _dateTimeProvider.UtcNow,
            LinkUrl: null,
            MetadataJson: JsonSerializer.Serialize(metadata));

        await _notificationPublisher.PublishAsync(notificationRequestedMessage, cancellationToken);
    }
}
