using NutriTrack.Contracts.Common;

namespace NutriTrack.Contracts.Reports;

public sealed record ListReportRunsResponse(PagedResponse<ReportRunResponse> Runs);
