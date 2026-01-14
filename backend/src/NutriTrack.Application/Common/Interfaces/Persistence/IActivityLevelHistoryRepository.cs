using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IActivityLevelHistoryRepository : IRepository<ActivityLevelHistoryEntry, ActivityLevelHistoryId>
{
    Task<ActivityLevelHistoryEntry?> GetClosestOnOrBeforeAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default);
}
