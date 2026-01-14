using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.Meals;

public sealed class MealSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public MealSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 120;

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
                throw new InvalidOperationException($"User '{username}' not found, run UserSeeder first!");

            return id;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        DateTimeOffset LocalAt(DateOnly date, int hour, int minute = 0)
            => new(date.ToDateTime(new TimeOnly(hour, minute)), TimeSpan.Zero);

        // 4–6 users as requested.
        var targetUsers = new[] { "desktop", "mobile", "user", "admin", "mia_lopez", "liamchen" };
        var targetUserIds = targetUsers.Select(GetUserId).ToArray();

        // Idempotency: (UserId, LocalDate, Name, OccurredAtLocal)
        var existing = await _dbContext.Meals
            .Where(m => targetUserIds.Contains(m.UserId) && m.LocalDate >= today.AddDays(-7) && m.LocalDate <= today)
            .Select(m => new { m.UserId, m.LocalDate, m.Name, m.OccurredAtLocal })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(x => (x.UserId, x.LocalDate, x.Name, x.OccurredAtLocal))
            .ToHashSet();

        var toInsert = new List<Meal>();

        // Seed last 7 days.
        for (var d = 6; d >= 0; d--)
        {
            var localDate = today.AddDays(-d);

            foreach (var username in targetUsers)
            {
                var userId = GetUserId(username);

                AddMealIfMissing(userId, localDate, "Breakfast", LocalAt(localDate, 8));
                AddMealIfMissing(userId, localDate, "Lunch", LocalAt(localDate, 13));
                AddMealIfMissing(userId, localDate, "Dinner", LocalAt(localDate, 19));

                // Snacks a few times per week.
                if (localDate.DayNumber % 3 == 0)
                {
                    AddMealIfMissing(userId, localDate, "Snack", LocalAt(localDate, 16));
                }
            }
        }

        if (toInsert.Count > 0)
        {
            _dbContext.Meals.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);

        void AddMealIfMissing(UserId userId, DateOnly localDate, string name, DateTimeOffset occurredAtLocal)
        {
            var key = (userId, localDate, name, occurredAtLocal);
            if (existingSet.Contains(key))
            {
                return;
            }

            var meal = Meal.Create(
                userId: userId,
                name: name,
                occurredAtUtc: occurredAtLocal.UtcDateTime,
                occurredAtLocal: occurredAtLocal,
                localDate: localDate,
                description: null);

            existingSet.Add(key);
            toInsert.Add(meal);
        }
    }
}
