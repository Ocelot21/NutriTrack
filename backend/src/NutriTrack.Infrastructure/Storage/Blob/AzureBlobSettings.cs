namespace NutriTrack.Infrastructure.Storage.Blob;

public sealed class AzureBlobSettings
{
    public const string SectionName = "AzureBlob";

    public string ConnectionString { get; init; } = string.Empty;

    public ContainersSettings Containers { get; init; } = new();

    public int SasExpiryMinutes { get; init; } = 60;

    public sealed class ContainersSettings
    {
        public string Avatars { get; init; } = "avatars";
        public string Groceries { get; init; } = "groceries";
        public string Exercises { get; init; } = "exercises";
        public string MealPhotos { get; init; } = "meal-photos";
        public string Reports { get; init; } = "reports";
    }
}
