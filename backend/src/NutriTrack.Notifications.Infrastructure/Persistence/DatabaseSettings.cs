namespace NutriTrack.Notifications.Infrastructure.Persistence;

public sealed class DatabaseSettings
{
    public const string SectionName = "Database";
    public string ConnectionString { get; init; } = string.Empty;
}