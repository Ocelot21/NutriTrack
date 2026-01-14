using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class ReportRunRepository : EfRepository<ReportRun, ReportRunId>, IReportRunRepository
{
    public ReportRunRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedResult<ReportRun>> ListForUserAsync(
        UserId requestedBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var query = _dbContext.ReportRuns
            .AsNoTracking()
            .Where(x => x.RequestedBy == requestedBy)
            .OrderByDescending(x => x.RequestedAtUtc);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ReportRun>(items, total, page, pageSize);
    }

    public async Task<IReadOnlyList<ReportRun>> ListQueuedAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0) take = 50;

        return await _dbContext.ReportRuns
            .Where(x => x.Status == ReportRunStatus.Queued)
            .OrderBy(x => x.RequestedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
