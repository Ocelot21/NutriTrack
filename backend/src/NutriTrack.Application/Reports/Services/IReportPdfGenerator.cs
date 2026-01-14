using NutriTrack.Domain.Reports;

namespace NutriTrack.Application.Reports.Services;

public interface IReportPdfGenerator
{
    Task<(string FileName, byte[] PdfBytes)> GenerateAsync(
        ReportRun reportRun,
        CancellationToken cancellationToken = default);
}
