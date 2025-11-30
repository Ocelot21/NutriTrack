using Microsoft.EntityFrameworkCore;
using NutriTrack.Infrastructure.Persistence.Seed;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Api.Startup;

public static class MigrationExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        var runner = sp.GetRequiredService<SeedRunner>();
        await runner.RunAsync();
    }
}
