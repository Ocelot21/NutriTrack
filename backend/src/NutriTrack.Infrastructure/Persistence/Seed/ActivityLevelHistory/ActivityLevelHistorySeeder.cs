using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.ActivityLevelHistory;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.ActivityLevelHistory;

public sealed class ActivityLevelHistorySeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public ActivityLevelHistorySeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 110;

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
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-120), ActivityLevel.Light),
                    (today.AddDays(-75), ActivityLevel.Moderate),
                }
            },
            new
            {
                Username = "mobile",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-90), ActivityLevel.Moderate),
                    (today.AddDays(-45), ActivityLevel.Active),
                }
            },
            new
            {
                Username = "user",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-30), ActivityLevel.Sedentary),
                    (today.AddDays(-7), ActivityLevel.Light),
                }
            },
            new
            {
                Username = "admin",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-180), ActivityLevel.Sedentary),
                    (today.AddDays(-60), ActivityLevel.Light),
                }
            },

            new
            {
                Username = "avabrown",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-150), ActivityLevel.Moderate),
                    (today.AddDays(-60), ActivityLevel.VeryActive),
                }
            },
            new
            {
                Username = "liamchen",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-120), ActivityLevel.Light),
                    (today.AddDays(-35), ActivityLevel.Moderate),
                }
            },
            new
            {
                Username = "mia_lopez",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-120), ActivityLevel.Sedentary),
                    (today.AddDays(-70), ActivityLevel.Light),
                }
            },
            new
            {
                Username = "oliver.khan",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-90), ActivityLevel.Sedentary),
                    (today.AddDays(-21), ActivityLevel.Light),
                }
            },
            new
            {
                Username = "sophia.ivanova",
                Changes = new (DateOnly Date, ActivityLevel Level)[]
                {
                    (today.AddDays(-200), ActivityLevel.Light),
                    (today.AddDays(-55), ActivityLevel.Moderate),
                }
            },
        };

        var targetUserIds = plans.Select(p => GetUserId(p.Username)).ToArray();

        var existing = await _dbContext.ActivityLevelHistoryEntries
            .Where(x => targetUserIds.Contains(x.UserId))
            .Select(x => new { x.UserId, x.EffectiveFrom })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(e => (e.UserId, e.EffectiveFrom))
            .ToHashSet();

        var toInsert = new List<ActivityLevelHistoryEntry>();

        foreach (var plan in plans)
        {
            var userId = GetUserId(plan.Username);

            foreach (var c in plan.Changes.OrderBy(x => x.Date))
            {
                if (c.Date > today)
                {
                    continue;
                }

                if (existingSet.Contains((userId, c.Date)))
                {
                    continue;
                }

                toInsert.Add(ActivityLevelHistoryEntry.Create(
                    userId: userId,
                    activityLevel: c.Level,
                    effectiveFrom: c.Date,
                    utcNow: utcNow));
            }
        }

        if (toInsert.Count > 0)
        {
            _dbContext.ActivityLevelHistoryEntries.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
