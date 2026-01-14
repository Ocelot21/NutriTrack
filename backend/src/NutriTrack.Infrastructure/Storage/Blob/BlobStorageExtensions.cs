using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.Common.Interfaces.Storage;

namespace NutriTrack.Infrastructure.Storage.Blob;

public static class BlobStorageExtensions
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobSettings = new AzureBlobSettings();
        configuration.Bind(AzureBlobSettings.SectionName, blobSettings);

        services.Configure<AzureBlobSettings>(configuration.GetSection(AzureBlobSettings.SectionName));
        services.AddSingleton(blobSettings);

        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        return services;
    }
}