using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.UserGoals;

public sealed class UserGoalSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public UserGoalSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 90;

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

        var goals = new (string Username, NutritionGoal Type, DateOnly StartDate, DateOnly TargetDate, decimal StartWeightKg, decimal TargetWeightKg)[]
        {
            ("desktop", NutritionGoal.MaintainWeight, today.AddDays(-28), today.AddDays(56), 82.0m, 82.0m),

            ("mobile", NutritionGoal.LoseWeight, today.AddDays(-21), today.AddDays(49), 60.0m, 56.5m),

            ("user", NutritionGoal.LoseWeight, today.AddDays(-14), today.AddDays(70), 85.0m, 78.0m),

            ("admin", NutritionGoal.MaintainWeight, today.AddDays(-10), today.AddDays(20), 78.0m, 78.0m),

            ("avabrown", NutritionGoal.GainWeight, today.AddDays(-35), today.AddDays(49), 55.0m, 58.0m),

            ("liamchen", NutritionGoal.LoseWeight, today.AddDays(-28), today.AddDays(56), 90.0m, 84.0m),

            ("noah.johnson", NutritionGoal.MaintainWeight, today.AddDays(-21), today.AddDays(35), 68.0m, 68.0m),

            ("mia_lopez", NutritionGoal.LoseWeight, today.AddDays(-42), today.AddDays(42), 62.0m, 58.5m),

            ("oliver.khan", NutritionGoal.GainWeight, today.AddDays(-14), today.AddDays(84), 92.0m, 96.0m),

            ("sophia.ivanova", NutritionGoal.MaintainWeight, today.AddDays(-30), today.AddDays(60), 70.0m, 70.0m),

            ("ethan_ng", NutritionGoal.MaintainWeight, today.AddDays(-18), today.AddDays(54), 76.0m, 76.0m),
        };

        var existing = await _dbContext.UserGoals
            .Select(g => new { g.UserId, g.Type, g.StartDate, g.TargetDate })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(e => (e.UserId, e.Type, e.StartDate, e.TargetDate))
            .ToHashSet();

        var toInsert = new List<UserGoal>();

        foreach (var g in goals)
        {
            var userId = GetUserId(g.Username);

            if (existingSet.Contains((userId, g.Type, g.StartDate, g.TargetDate)))
            {
                continue;
            }

            toInsert.Add(UserGoal.Create(
                userId: userId,
                type: g.Type,
                startDate: g.StartDate,
                targetDate: g.TargetDate,
                startWeightKg: g.StartWeightKg,
                targetWeightKg: g.TargetWeightKg));
        }

        if (toInsert.Count > 0)
        {
            _dbContext.UserGoals.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
