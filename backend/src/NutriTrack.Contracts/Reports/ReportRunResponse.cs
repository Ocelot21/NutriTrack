namespace NutriTrack.Contracts.Reports;

public sealed record ReportRunResponse(
    Guid Id,
    int Type,
    int Status,
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
