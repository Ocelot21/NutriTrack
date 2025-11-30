using NutriTrack.Application.Common.Interfaces.Services;

namespace NutriTrack.Infrastructure.Services
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
