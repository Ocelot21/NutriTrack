namespace NutriTrack.Contracts.Reports;

public sealed record CreateReportRunRequest(
    int Type,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    object? Parameters
);
