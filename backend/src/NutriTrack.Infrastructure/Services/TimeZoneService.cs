using NodaTime;
using NodaTime.TimeZones;
using NutriTrack.Application.Common.Interfaces.Services;

namespace NutriTrack.Infrastructure.Services;

public sealed class TimeZoneService : ITimeZoneService
{
    private static readonly IDateTimeZoneProvider _dateTimeZoneProvider = NodaTime.DateTimeZoneProviders.Tzdb;

    public bool TryNormalize(string timeZoneId, out string normalizedTimeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            normalizedTimeZoneId = default!;
            return false;
        }
        var zone = _dateTimeZoneProvider.GetZoneOrNull(timeZoneId.Trim());
        if (zone is null)
        {
            normalizedTimeZoneId = default!;
            return false;
        }

        normalizedTimeZoneId = zone.Id;
        return true;
    }

    public DateOnly LocalDate(DateTime utcDateTime, string timeZoneId)
    {
        return DateOnly.FromDateTime(ToLocal(utcDateTime, timeZoneId));
    }

    public int LocalTimeMinutes(DateTime utcDateTime, string timeZoneId)
    {
        var localDateTime = ToLocal(utcDateTime, timeZoneId);
        return (localDateTime.Hour * 60) + localDateTime.Minute;
    }

    public short OffsetMinutes(string timeZoneId, DateTime utcDateTime)
    {
        var dateTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc));
        var zone = _dateTimeZoneProvider[timeZoneId];
        var offset = zone.GetUtcOffset(dateTimeUtc);
        return (short)(offset.Seconds / 60);
    }

    public DateTime ToLocal(DateTime utcDateTime, string timeZoneId)
    {
        var dateTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc));
        var zone = _dateTimeZoneProvider[timeZoneId];
        var zonedDateTime = dateTimeUtc.InZone(zone);
        return zonedDateTime.ToDateTimeUnspecified();
    }
}
