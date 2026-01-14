using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Social.Snapshots;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class DailyOverviewSnapshotRepository : IDailyOverviewSnapshotRepository
{
    private readonly AppDbContext _dbContext;

    public DailyOverviewSnapshotRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<DailyOverviewSnapshot?> GetByIdAsync(DailyOverviewSnapshotId id, CancellationToken cancellationToken = default)
        => _dbContext.DailyOverviewSnapshots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(DailyOverviewSnapshot snapshot, CancellationToken cancellationToken = default)
        => _dbContext.DailyOverviewSnapshots.AddAsync(snapshot, cancellationToken).AsTask();

    public void Remove(DailyOverviewSnapshot snapshot)
        => _dbContext.DailyOverviewSnapshots.Remove(snapshot);
}
