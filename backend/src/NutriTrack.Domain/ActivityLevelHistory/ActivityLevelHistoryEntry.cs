using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.ActivityLevelHistory;

public sealed class ActivityLevelHistoryEntry : AggregateRoot<ActivityLevelHistoryId>
{
    public UserId UserId { get; private set; }

    public User? User { get; private set; } = null!;

    public ActivityLevel ActivityLevel { get; private set; }

    public DateOnly EffectiveFrom { get; private set; }

    private ActivityLevelHistoryEntry() : base()
    {
    }

    private ActivityLevelHistoryEntry(
        ActivityLevelHistoryId id,
        UserId userId,
        ActivityLevel activityLevel,
        DateOnly effectiveFrom) : base(id)
    {
        UserId = userId;
        ActivityLevel = activityLevel;
        EffectiveFrom = effectiveFrom;
    }

    public static ActivityLevelHistoryEntry Create(
        UserId userId,
        ActivityLevel activityLevel,
        DateOnly effectiveFrom,
        DateTime utcNow)
    {
        var id = new ActivityLevelHistoryId(Guid.NewGuid());

        var entry = new ActivityLevelHistoryEntry(
            id,
            userId,
            activityLevel,
            effectiveFrom);

        entry.SetCreated(utcNow, userId);

        return entry;
    }
}
