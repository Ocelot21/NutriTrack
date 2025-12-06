using NutriTrack.Notifications.Application.Common.Interfaces.Services;

namespace NutriTrack.Notifications.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
