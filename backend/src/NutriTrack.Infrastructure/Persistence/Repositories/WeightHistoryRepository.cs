using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class WeightHistoryRepository : EfRepository<WeightHistoryEntry, WeightHistoryEntryId>, IWeightHistoryRepository
{
    public WeightHistoryRepository(AppDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<IReadOnlyList<WeightHistoryEntry>> GetInRangeForUser(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var entries = await _dbContext.WeightHistoryEntries
            .Where(e => e.UserId == userId && e.Date >= from && e.Date <= to)
            .ToListAsync(cancellationToken);

        return entries;
    }
}
