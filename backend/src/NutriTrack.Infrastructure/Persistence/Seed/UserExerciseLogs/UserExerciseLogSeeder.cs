using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Seed.UserExerciseLogs;

public sealed class UserExerciseLogSeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public UserExerciseLogSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 140;

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

        var exercises = await _dbContext.Exercises
            .Select(e => new { e.Id, e.Name })
            .ToListAsync(cancellationToken);

        var exerciseIdByName = exercises
            .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        async Task<Exercise> GetExerciseAsync(string name)
        {
            if (!exerciseIdByName.TryGetValue(name, out var id))
                throw new InvalidOperationException($"Exercise '{name}' not found, run ExerciseSeeder first!");

            return await _dbContext.Exercises.SingleAsync(e => e.Id == id, cancellationToken);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        DateTimeOffset LocalAt(DateOnly date, int hour, int minute = 0)
        {
            var dt = date.ToDateTime(new TimeOnly(hour, minute));
            return new DateTimeOffset(dt, TimeSpan.Zero);
        }

        var targetUsers = new[] { "desktop", "mobile", "user", "admin", "mia_lopez", "liamchen" };
        var targetUserIds = targetUsers.Select(GetUserId).ToArray();

        var existing = await _dbContext.UserExerciseLogs
            .Where(x => targetUserIds.Contains(x.UserId))
            .Select(x => new { x.UserId, x.LocalDate, x.ExerciseId, x.DurationMinutes })
            .ToListAsync(cancellationToken);

        var existingSet = existing
            .Select(x => (x.UserId, x.LocalDate, x.ExerciseId, x.DurationMinutes))
            .ToHashSet();

        var toInsert = new List<UserExerciseLog>();

        // Seed last 45 days with realistic frequency:
        // - mobile: frequent cardio
        // - desktop: moderate mix
        // - user/admin: occasional
        // - mia/lliam: moderate
        for (var d = 44; d >= 0; d--)
        {
            var date = today.AddDays(-d);

            foreach (var username in targetUsers)
            {
                var userId = GetUserId(username);
                var dayKey = date.DayNumber;

                if (string.Equals(username, "mobile", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 2 == 0)
                    {
                        await AddIfMissing(userId, date, "Jogging", 35m, LocalAt(date, 18), "Evening run");
                    }
                    else if (dayKey % 3 == 0)
                    {
                        await AddIfMissing(userId, date, "Cycling (leisure)", 45m, LocalAt(date, 17), "Stationary bike");
                    }
                    else if (dayKey % 5 == 0)
                    {
                        await AddIfMissing(userId, date, "HIIT - High Knees", 18m, LocalAt(date, 19), "Short HIIT session");
                    }
                }
                else if (string.Equals(username, "desktop", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 3 == 0)
                    {
                        await AddIfMissing(userId, date, "Strength - Full Body", 50m, LocalAt(date, 19), "Gym session");
                    }
                    else if (dayKey % 5 == 0)
                    {
                        await AddIfMissing(userId, date, "Walking", 40m, LocalAt(date, 12), "Lunch break walk");
                    }
                    else if (dayKey % 7 == 0)
                    {
                        await AddIfMissing(userId, date, "Rowing (moderate)", 22m, LocalAt(date, 18), null);
                    }
                }
                else if (string.Equals(username, "liamchen", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 4 == 0)
                    {
                        await AddIfMissing(userId, date, "Cycling (vigorous)", 30m, LocalAt(date, 18), "Intervals");
                    }
                    else if (dayKey % 6 == 0)
                    {
                        await AddIfMissing(userId, date, "Rowing (moderate)", 25m, LocalAt(date, 18), null);
                    }
                    else if (dayKey % 9 == 0)
                    {
                        await AddIfMissing(userId, date, "Hiking", 60m, LocalAt(date, 10), "Weekend hike");
                    }
                }
                else if (string.Equals(username, "mia_lopez", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 3 == 1)
                    {
                        await AddIfMissing(userId, date, "Walking", 45m, LocalAt(date, 9), "Morning walk");
                    }
                    else if (dayKey % 7 == 0)
                    {
                        await AddIfMissing(userId, date, "Yoga (vinyasa)", 35m, LocalAt(date, 20), null);
                    }
                    else if (dayKey % 11 == 0)
                    {
                        await AddIfMissing(userId, date, "Pilates", 40m, LocalAt(date, 19), null);
                    }
                }
                else if (string.Equals(username, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 7 == 0)
                    {
                        await AddIfMissing(userId, date, "Walking", 30m, LocalAt(date, 8), "Daily steps");
                    }
                }
                else if (string.Equals(username, "user", StringComparison.OrdinalIgnoreCase))
                {
                    if (dayKey % 5 == 2)
                    {
                        await AddIfMissing(userId, date, "Stretching", 20m, LocalAt(date, 21), null);
                    }
                    else if (dayKey % 13 == 0)
                    {
                        await AddIfMissing(userId, date, "Dynamic Warm-up", 15m, LocalAt(date, 20), "Getting started");
                    }
                }
            }
        }

        if (toInsert.Count > 0)
        {
            _dbContext.UserExerciseLogs.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);

        async Task AddIfMissing(UserId userId, DateOnly localDate, string exerciseName, decimal durationMinutes, DateTimeOffset occurredAtLocal, string? notes)
        {
            // Map a couple of composite labels to real seeded exercises.
            // (Keeps the seed data readable while still using Exercise entities.)
            var canonicalExerciseName = exerciseName switch
            {
                "Strength - Full Body" => "Back Squat",
                _ => exerciseName,
            };

            var exercise = await GetExerciseAsync(canonicalExerciseName);
            var key = (userId, localDate, exercise.Id, durationMinutes);

            if (existingSet.Contains(key))
            {
                return;
            }

            var occurredAtUtc = occurredAtLocal.UtcDateTime;

            var log = UserExerciseLog.Create(
                userId: userId,
                exercise: exercise,
                durationMinutes: durationMinutes,
                occurredAtUtc: occurredAtUtc,
                occurredAtLocal: occurredAtLocal,
                localDate: localDate,
                notes: notes);

            existingSet.Add(key);
            toInsert.Add(log);
        }
    }
}
