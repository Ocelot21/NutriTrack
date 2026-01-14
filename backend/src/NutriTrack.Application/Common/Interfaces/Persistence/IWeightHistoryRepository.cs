using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IWeightHistoryRepository : IRepository<WeightHistoryEntry, WeightHistoryEntryId>
{
    Task<IReadOnlyList<WeightHistoryEntry>> GetInRangeForUser(
        UserId userId,
        DateOnly From,
        DateOnly To,
        CancellationToken cancellationToken = default);

    Task<WeightHistoryEntry?> GetClosestOnOrBeforeAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
