using NutriTrack.Domain.Reports;

namespace NutriTrack.Application.Reports.Common;

public sealed record ReportRunResult(
    ReportRunId Id,
    ReportType Type,
    ReportRunStatus Status,
    DateTime RequestedAtUtc,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    string? OutputPdfUri,
    string? OutputPdfBlobName,
    string? OutputFileName,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason
);
