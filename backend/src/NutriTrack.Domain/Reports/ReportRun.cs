using System.Text.Json;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Reports;

public sealed class ReportRun : AggregateRoot<ReportRunId>
{
    private ReportRun()
    {
        // EF Core
    }

    private ReportRun(
        ReportRunId id,
        ReportType type,
        ReportRunStatus status,
        UserId requestedBy,
        DateTime requestedAtUtc,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        string? parametersJson)
        : base(id)
    {
        Type = type;
        Status = status;
        RequestedBy = requestedBy;
        RequestedAtUtc = requestedAtUtc;
        FromUtc = fromUtc;
        ToUtc = toUtc;
        ParametersJson = parametersJson;
    }

    public ReportType Type { get; private set; }

    public ReportRunStatus Status { get; private set; }

    public UserId RequestedBy { get; private set; }

    public DateTime RequestedAtUtc { get; private set; }

    public DateTimeOffset FromUtc { get; private set; }

    public DateTimeOffset ToUtc { get; private set; }

    public string? ParametersJson { get; private set; }

    public string? OutputPdfUri { get; private set; }

    public string? OutputPdfBlobName { get; private set; }

    public string? OutputFileName { get; private set; }

    public DateTime? StartedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public string? FailureReason { get; private set; }

    public static ReportRun Create(
        ReportRunId id,
        ReportType type,
        UserId requestedBy,
        DateTime requestedAtUtc,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        object? parameters = null)
    {
        var json = parameters is null
            ? null
            : JsonSerializer.Serialize(parameters, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        return new ReportRun(
            id,
            type,
            ReportRunStatus.Queued,
            requestedBy,
            requestedAtUtc,
            fromUtc,
            toUtc,
            json);
    }

    public void MarkRunning(DateTime utcNow)
    {
        if (Status != ReportRunStatus.Queued)
        {
            return;
        }

        Status = ReportRunStatus.Running;
        StartedAtUtc = utcNow;
        FailureReason = null;
    }

    public void MarkReady(DateTime utcNow, string pdfBlobName, string fileName)
    {
        Status = ReportRunStatus.Ready;
        CompletedAtUtc = utcNow;
        OutputPdfBlobName = pdfBlobName;
        OutputFileName = fileName;
        FailureReason = null;
    }

    public void MarkFailed(DateTime utcNow, string reason)
    {
        Status = ReportRunStatus.Failed;
        CompletedAtUtc = utcNow;
        FailureReason = string.IsNullOrWhiteSpace(reason) ? "Unknown" : reason;
    }
}
