namespace NutriTrack.Domain.Notifications;

public sealed class NotificationReadModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public string? LinkUrl { get; set; }
    public string? MetadataJson { get; set; }
}
