namespace NutriTrack.Contracts.ActivityLevelHistoryEntries;

public sealed record CreateActivityLevelHistoryEntryRequest(
    DateOnly EffectiveFrom,
    string ActivityLevel
);
