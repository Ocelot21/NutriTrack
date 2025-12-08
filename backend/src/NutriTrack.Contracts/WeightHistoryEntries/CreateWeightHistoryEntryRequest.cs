namespace NutriTrack.Contracts.WeightHistoryEntries;

public sealed record CreateWeightHistoryEntryRequest(
    DateOnly Date,
    decimal WeightKg
);
