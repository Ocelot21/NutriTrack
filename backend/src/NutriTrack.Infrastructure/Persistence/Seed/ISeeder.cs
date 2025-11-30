namespace NutriTrack.Infrastructure.Persistence.Seed;

public interface ISeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
    int Order { get; }
}
