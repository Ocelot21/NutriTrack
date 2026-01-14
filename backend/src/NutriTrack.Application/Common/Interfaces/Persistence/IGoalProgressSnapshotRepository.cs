using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IGoalProgressSnapshotRepository
{
    Task<GoalProgressSnapshot?> GetByIdAsync(GoalProgressSnapshotId id, CancellationToken cancellationToken = default);
    Task AddAsync(GoalProgressSnapshot snapshot, CancellationToken cancellationToken = default);
    void Remove(GoalProgressSnapshot snapshot);
}
