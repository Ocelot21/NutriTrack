namespace NutriTrack.Application.Reports.Services;

public interface IReportOutputStorage
{
    Task<(string BlobName, string ReadUri)> StorePdfAsync(
        byte[] pdfBytes,
        string fileName,
        CancellationToken cancellationToken = default);
}
