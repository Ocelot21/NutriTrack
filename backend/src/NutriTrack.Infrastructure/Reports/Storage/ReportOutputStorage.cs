using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Reports.Services;

namespace NutriTrack.Infrastructure.Reports.Storage;

public sealed class ReportOutputStorage : IReportOutputStorage
{
    private readonly IBlobStorageService _blobStorage;

    public ReportOutputStorage(IBlobStorageService blobStorage)
    {
        _blobStorage = blobStorage;
    }

    public async Task<(string BlobName, string ReadUri)> StorePdfAsync(
        byte[] pdfBytes,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (pdfBytes is null || pdfBytes.Length == 0)
            throw new ArgumentException("PDF content is empty.", nameof(pdfBytes));

        using var ms = new MemoryStream(pdfBytes);

        var blobName = await _blobStorage.UploadAsync(
            container: BlobContainer.Reports,
            content: ms,
            fileName: fileName,
            contentType: "application/pdf",
            cancellationToken: cancellationToken);

        var readUri = _blobStorage.GenerateReadUri(BlobContainer.Reports, blobName)
            ?? throw new InvalidOperationException("Failed to generate report read URI.");

        return (blobName, readUri);
    }
}
