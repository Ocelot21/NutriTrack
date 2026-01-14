using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Notifications;
using NutriTrack.Application.Notifications.Messages;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Exercises.Events;
using System.Text.Json;

using AppNotificationPublisher = NutriTrack.Application.Common.Interfaces.Messaging.INotificationPublisher;

namespace NutriTrack.Application.Exercises.EventHandlers;

public sealed class ExerciseSuggestionApprovedDomainEventHandler
    : INotificationHandler<DomainEventNotification<ExerciseSuggestionApprovedDomainEvent>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly AppNotificationPublisher _notificationPublisher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ExerciseSuggestionApprovedDomainEventHandler(
        IExerciseRepository exerciseRepository,
        AppNotificationPublisher notificationPublisher,
        IDateTimeProvider dateTimeProvider)
    {
        _exerciseRepository = exerciseRepository;
        _notificationPublisher = notificationPublisher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        DomainEventNotification<ExerciseSuggestionApprovedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var exercise = await _exerciseRepository.GetByIdAsync(domainEvent.ExerciseId, cancellationToken);
        if (exercise is null)
        {
            return;
        }

        var title = "Suggestion approved";
        var message = $"Your exercise suggestion '{exercise.Name}' was approved.";

        var metadata = new
        {
            suggestionType = "Exercise",
            exerciseId = exercise.Id.Value,
            exerciseName = exercise.Name
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
