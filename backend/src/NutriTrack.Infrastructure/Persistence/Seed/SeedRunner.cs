using Microsoft.Extensions.Logging;

namespace NutriTrack.Infrastructure.Persistence.Seed;

public sealed class SeedRunner
{
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly ILogger<SeedRunner> _logger;

    public SeedRunner(IEnumerable<ISeeder> seeders, ILogger<SeedRunner> logger)
    {
        _seeders = seeders.OrderBy(s => s.Order).ToArray();
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        foreach (var seeder in _seeders)
        {
            _logger.LogInformation("Seeding: {Seeder}", seeder.GetType().Name);
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
