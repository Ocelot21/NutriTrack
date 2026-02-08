using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Infrastructure.Persistence.Seed.WeightHistory;

public sealed class WeightHistorySeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public WeightHistorySeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 100;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var users = await _dbContext.Users
            .Select(u => new { u.Id, Username = u.Username.Value })
            .ToListAsync(cancellationToken);

        var userIdByUsername = users.ToDictionary(u => u.Username, u => u.Id, StringComparer.OrdinalIgnoreCase);

        UserId GetUserId(string username)
        {
            if (!userIdByUsername.TryGetValue(username, out var id))
            {
                throw new InvalidOperationException($"User '{username}' not found, run UserSeeder first!");
            }

            return id;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var utcNow = DateTime.UtcNow;

        var plans = new[]
        {
            new
            {
                Username = "desktop",
                StartDaysAgo = 70,
                IntervalDays = 7,
                Weights = new decimal[] { 83.2m, 83.0m, 82.8m, 82.6m, 82.5m, 82.3m, 82.2m, 82.1m, 82.0m, 82.1m, 82.0m }
            },
            new
            {
                Username = "mobile",
                StartDaysAgo = 56,
                IntervalDays = 7,
                Weights = new decimal[] { 61.2m, 60.9m, 60.4m, 60.0m, 59.6m, 59.1m, 58.7m, 58.2m, 57.8m }
            },
            new
            {
                Username = "user",
                StartDaysAgo = 42,
                IntervalDays = 7,
                Weights = new decimal[] { 87.5m, 87.2m, 86.8m, 86.4m, 86.0m, 85.6m, 85.2m }
            },
            new
            {
                Username = "admin",
                StartDaysAgo = 35,
                IntervalDays = 7,
                Weights = new decimal[] { 78.6m, 78.3m, 78.1m, 78.2m, 78.0m, 78.1m }
            },

            new
            {
                Username = "avabrown",
                StartDaysAgo = 70,
                IntervalDays = 7,
                Weights = new decimal[] { 54.8m, 55.1m, 55.3m, 55.7m, 56.0m, 56.2m, 56.6m, 56.9m, 57.2m, 57.5m, 57.8m }
            },
            new
            {
                Username = "liamchen",
                StartDaysAgo = 63,
                IntervalDays = 7,
                Weights = new decimal[] { 92.0m, 91.3m, 90.6m, 90.0m, 89.4m, 88.8m, 88.1m, 87.6m, 87.0m, 86.5m }
            },
            new
            {
                Username = "mia_lopez",
                StartDaysAgo = 77,
                IntervalDays = 7,
                Weights = new decimal[] { 63.2m, 63.0m, 62.7m, 62.3m, 62.0m, 61.7m, 61.4m, 61.0m, 60.6m, 60.2m, 59.9m, 59.5m }
            },
            new
            {
                Username = "oliver.khan",
                StartDaysAgo = 56,
                IntervalDays = 7,
                Weights = new decimal[] { 91.8m, 92.0m, 92.3m, 92.6m, 92.9m, 93.2m, 93.5m, 93.9m, 94.2m }
            },
            new
            {
                Username = "sophia.ivanova",
                StartDaysAgo = 63,
                IntervalDays = 7,
                Weights = new decimal[] { 70.8m, 70.5m, 70.2m, 70.1m, 70.0m, 70.2m, 70.1m, 70.0m, 70.1m, 70.0m }
            },
        };

        var targetUserIds = plans.Select(p => GetUserId(p.Username)).ToArray();

        var existing = await _dbContext.WeightHistoryEntries
            .Where(x => targetUserIds.Contains(x.UserId))
            .Select(x => new { x.UserId, x.Date })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(e => (e.UserId, e.Date))
            .ToHashSet();

        var toInsert = new List<WeightHistoryEntry>();

        foreach (var plan in plans)
        {
            var userId = GetUserId(plan.Username);
            var startDate = today.AddDays(-plan.StartDaysAgo);

            for (var i = 0; i < plan.Weights.Length; i++)
            {
                var date = startDate.AddDays(i * plan.IntervalDays);

                if (date > today)
                {
                    break;
                }

                if (existingSet.Contains((userId, date)))
                {
                    continue;
                }

                toInsert.Add(WeightHistoryEntry.Create(
                    userId: userId,
                    date: date,
                    weightKg: plan.Weights[i],
                    utcNow: utcNow));
            }
        }

        if (toInsert.Count > 0)
        {
            _dbContext.WeightHistoryEntries.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
