using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Infrastructure.Storage.Blob;

public sealed class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureBlobSettings _settings;

    private readonly Dictionary<BlobContainer, BlobContainerClient> _containers = new();

    public BlobStorageService(IOptions<AzureBlobSettings> options)
    {
        _settings = options.Value;

        if (string.IsNullOrWhiteSpace(_settings.ConnectionString))
            throw new InvalidOperationException("AzureBlob:ConnectionString is missing.");

        _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
    }

    public async Task<string> UploadAsync(
        BlobContainer container,
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (content is null) throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName is required.", nameof(fileName));
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("contentType is required.", nameof(contentType));

        var containerClient = GetContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = GuessExtensionFromContentType(contentType);

        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(content, options, cancellationToken);
        return blobName;
    }

    public async Task DeleteAsync(
        BlobContainer container,
        string? blobName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return;

        var containerClient = GetContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public string? GenerateReadUri(
        BlobContainer container,
        string? blobName,
        TimeSpan? lifetime = null)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return null;

        var containerClient = GetContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        var expires = DateTimeOffset.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(_settings.SasExpiryMinutes));

        var sas = new BlobSasBuilder
        {
            BlobContainerName = containerClient.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expires
        };

        sas.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sas);
        return sasUri.ToString();
    }

    private BlobContainerClient GetContainerClient(BlobContainer container)
    {
        if (_containers.TryGetValue(container, out var existing))
            return existing;

        var containerName = container switch
        {
            BlobContainer.Avatars => _settings.Containers.Avatars,
            BlobContainer.Groceries => _settings.Containers.Groceries,
            BlobContainer.Exercises => _settings.Containers.Exercises,
            BlobContainer.Reports => _settings.Containers.Reports,
            _ => throw new ArgumentOutOfRangeException(nameof(container), container, "Unknown container.")
        };

        if (string.IsNullOrWhiteSpace(containerName))
            throw new InvalidOperationException($"AzureBlob container name missing for: {container}");

        var client = _blobServiceClient.GetBlobContainerClient(containerName);
        _containers[container] = client;
        return client;
    }

    private static string GuessExtensionFromContentType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => string.Empty
        };
    }
}
