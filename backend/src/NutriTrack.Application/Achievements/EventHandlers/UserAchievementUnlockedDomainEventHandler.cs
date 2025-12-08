using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Notifications;
using NutriTrack.Application.Notifications.Messages;
using NutriTrack.Domain.Achievements.Events;
using System.Text.Json;

using AppNotificationPublisher = NutriTrack.Application.Common.Interfaces.Messaging.INotificationPublisher;

namespace NutriTrack.Application.Achievements.EventHandlers;

public sealed class UserAchievementUnlockedDomainEventHandler
    : INotificationHandler<DomainEventNotification<UserAchievementUnlockedDomainEvent>>
{
    private readonly IAchievementRepository _achievementRepository;
    private readonly AppNotificationPublisher _notificationPublisher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserAchievementUnlockedDomainEventHandler(
        IAchievementRepository achievementRepository,
        AppNotificationPublisher notificationPublisher,
        IDateTimeProvider dateTimeProvider)
    {
        _achievementRepository = achievementRepository;
        _notificationPublisher = notificationPublisher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        DomainEventNotification<UserAchievementUnlockedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var achievement = await _achievementRepository.GetByIdAsync(
            domainEvent.AchievementId,
            cancellationToken);

        if (achievement is null)
        {
            // Optional TODO: Log warning about missing achievement
            return;
        }

        var title = "Achievement unlocked 🎉";
        var message = $"{achievement.Title} (+{achievement.Points} points)";

        var metadata = new
        {
            achievementKey = achievement.Key,
            achievementTitle = achievement.Title,
            points = achievement.Points
        };

        var notificationRequestedMessage = new NotificationRequestedMessage(
            UserId: domainEvent.UserId.Value,
            Title: title,
            Message: message,
            Type: "AchievementUnlocked",
            OccurredAtUtc: _dateTimeProvider.UtcNow,
            LinkUrl: null,
            MetadataJson: JsonSerializer.Serialize(metadata)
        );

        await _notificationPublisher.PublishAsync(notificationRequestedMessage, cancellationToken);
    }
}