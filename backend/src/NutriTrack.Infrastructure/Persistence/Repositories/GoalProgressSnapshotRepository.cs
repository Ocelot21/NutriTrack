using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class GoalProgressSnapshotRepository : IGoalProgressSnapshotRepository
{
    private readonly AppDbContext _dbContext;

    public GoalProgressSnapshotRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<GoalProgressSnapshot?> GetByIdAsync(GoalProgressSnapshotId id, CancellationToken cancellationToken = default)
        => _dbContext.GoalProgressSnapshots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(GoalProgressSnapshot snapshot, CancellationToken cancellationToken = default)
        => _dbContext.GoalProgressSnapshots.AddAsync(snapshot, cancellationToken).AsTask();

    public void Remove(GoalProgressSnapshot snapshot)
        => _dbContext.GoalProgressSnapshots.Remove(snapshot);
}
