namespace NutriTrack.Application.Common.Interfaces.Services;

public interface ITimeZoneService
{
    bool TryNormalize(string timeZoneId, out string normalizedTimeZoneId);
    DateTime ToLocal(DateTime utcDateTime, string timeZoneId);
    DateOnly LocalDate(DateTime utcDateTime, string timeZoneId);
    int LocalTimeMinutes(DateTime utcDateTime, string timeZoneId);
    short OffsetMinutes(string timeZoneId, DateTime utcDateTime);
    DateTime ToUtc(DateTimeOffset localDateTime, string timeZoneId);
    IEnumerable<string> GetSystemTimeZoneIds();
}
