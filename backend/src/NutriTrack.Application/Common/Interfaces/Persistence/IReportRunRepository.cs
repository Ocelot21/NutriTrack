using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IReportRunRepository : IRepository<ReportRun, ReportRunId>
{
    Task<PagedResult<ReportRun>> ListForUserAsync(
        UserId requestedBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReportRun>> ListQueuedAsync(
        int take,
        CancellationToken cancellationToken = default);
}
