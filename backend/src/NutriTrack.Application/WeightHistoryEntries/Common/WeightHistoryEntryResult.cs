using NutriTrack.Domain.WeightHistory;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.WeightHistoryEntries.Common;

public sealed record WeightHistoryEntryResult(
    WeightHistoryEntryId Id,
    UserId UserId,
    DateOnly Date,
    decimal WeightKg
);
