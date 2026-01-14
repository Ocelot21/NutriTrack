using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Reports;

namespace NutriTrack.Application.Reports.Services;

public interface IReportDataService
{
    Task<WeeklyOverviewReportData> GetWeeklyOverviewAsync(
        ReportRun run,
        CancellationToken cancellationToken = default);

    Task<UserActivityReportData> GetUserActivityAsync(
        ReportRun run,
        CancellationToken cancellationToken = default);

    Task<AdminAuditReportData> GetAdminAuditAsync(
        ReportRun run,
        CancellationToken cancellationToken = default);
}
