using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Reports;

namespace NutriTrack.Application.Common.Mappings;

public static class ReportRunMappings
{
    public static ReportRunResult ToReportRunResult(this ReportRun run)
        => new(
            run.Id,
            run.Type,
            run.Status,
            run.RequestedAtUtc,
            run.FromUtc,
            run.ToUtc,
            null,
            run.OutputPdfBlobName,
            run.OutputFileName,
            run.StartedAtUtc,
            run.CompletedAtUtc,
            run.FailureReason);
}
