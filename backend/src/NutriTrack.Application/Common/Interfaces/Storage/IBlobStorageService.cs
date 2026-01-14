using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Common.Interfaces.Storage;

public interface IBlobStorageService
{
    Task<string> UploadAsync(
        BlobContainer container,
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        BlobContainer container,
        string? blobName,
        CancellationToken cancellationToken = default);

    string? GenerateReadUri(
        BlobContainer container,
        string? blobName,
        TimeSpan? lifetime = null);
}
