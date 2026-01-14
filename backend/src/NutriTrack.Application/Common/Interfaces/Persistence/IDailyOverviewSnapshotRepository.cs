using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IDailyOverviewSnapshotRepository
{
    Task<DailyOverviewSnapshot?> GetByIdAsync(DailyOverviewSnapshotId id, CancellationToken cancellationToken = default);
    Task AddAsync(DailyOverviewSnapshot snapshot, CancellationToken cancellationToken = default);
    void Remove(DailyOverviewSnapshot snapshot);
}
