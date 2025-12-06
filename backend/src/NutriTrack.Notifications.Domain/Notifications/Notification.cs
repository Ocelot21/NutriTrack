using NutriTrack.Notifications.Domain.Common.Models;
using NutriTrack.Notifications.Domain.Users;
using System;

namespace NutriTrack.Notifications.Domain.Notifications;

public sealed class Notification : AggregateRoot<NotificationId>
{
    public UserId UserId { get; private set; }

    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public NotificationType Type { get; private set; }

    public NotificationStatus Status { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }

    public string? LinkUrl { get; private set; }
    public string? MetadataJson { get; private set; }

    private Notification() : base()
    {
    }

    private Notification(
        NotificationId id,
        UserId userId,
        string title,
        string message,
        NotificationType type,
        DateTime occurredAtUtc,
        string? linkUrl,
        string? metadataJson) : base(id)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        Status = NotificationStatus.Unread;
        OccurredAtUtc = occurredAtUtc;
        LinkUrl = linkUrl;
        MetadataJson = metadataJson;
    }

    public static Notification Create(
        UserId userId,
        string title,
        string message,
        NotificationType type,
        DateTime occurredAtUtc,
        string? linkUrl = null,
        string? metadataJson = null)
    {
        var id = new NotificationId(Guid.NewGuid());

        var notification = new Notification(
            id,
            userId,
            title,
            message,
            type,
            occurredAtUtc,
            linkUrl,
            metadataJson);

        notification.RaiseDomainEvent(new NotificationCreatedDomainEvent(id, userId));

        notification.SetCreated(occurredAtUtc, null);

        return notification;
    }

    public void MarkAsRead(DateTime utcNow)
    {
        if (Status == NotificationStatus.Read)
            return;

        Status = NotificationStatus.Read;
        ReadAtUtc = utcNow;
        SetModified(utcNow, null);

        RaiseDomainEvent(new NotificationReadDomainEvent(Id, UserId, utcNow));
    }


    public void Archive(DateTime utcNow)
    {
        if (Status == NotificationStatus.Archived)
            return;

        Status = NotificationStatus.Archived;
        SetModified(utcNow, null);
    }
}
