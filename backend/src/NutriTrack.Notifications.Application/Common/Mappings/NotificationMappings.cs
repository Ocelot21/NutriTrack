using NutriTrack.Notifications.Application.Common.Models;
using NutriTrack.Notifications.Application.Notifications.Common;
using NutriTrack.Notifications.Domain.Notifications;

namespace NutriTrack.Notifications.Application.Common.Mappings;

public static class NotificationMappings
{
    public static NotificationResult ToNotificationResult(this Notification notification)
    {
        return new NotificationResult(
            notification.Id.Value,
                notification.Title,
                notification.Message,
                notification.Type.ToString(),
                notification.Status.ToString(),
                notification.OccurredAtUtc,
                notification.ReadAtUtc,
                notification.LinkUrl);
    }

    public static PagedResult<NotificationResult> ToPagedNotificationResult(
        this PagedResult<Notification> pagedNotifications)
    {
        var notificationResults = pagedNotifications.Items
            .Select(n => n.ToNotificationResult())
            .ToList();

        return new PagedResult<NotificationResult>(
            notificationResults,
            pagedNotifications.TotalCount,
            pagedNotifications.Page,
            pagedNotifications.PageSize);
    }
}