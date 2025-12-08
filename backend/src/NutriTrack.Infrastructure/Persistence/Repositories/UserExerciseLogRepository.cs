using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class UserExerciseLogRepository : EfRepository<UserExerciseLog, UserExerciseLogId>, IUserExerciseLogRepository
{
    public UserExerciseLogRepository(AppDbContext dbContext) : base(dbContext) { }

    public Task<int> CountLogsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserExerciseLogs.CountAsync(l => l.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserExerciseLog>> GetByUserAndDateRangeAsync(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
        => await _dbContext.UserExerciseLogs
            .Where(l => l.UserId == userId && l.LocalDate >= from && l.LocalDate <= to)
            .ToListAsync(cancellationToken);

    public async Task<int> GetCurrentStreakDaysAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var logs = await _dbContext.UserExerciseLogs
            .Where(l => l.UserId == userId && l.LocalDate <= date)
            .OrderByDescending(l => l.LocalDate)
            .ToListAsync(cancellationToken);

        if (!logs.Any())
        {
            return 0;
        }

        int streak = 0;
        DateOnly currentDate = date;

        foreach (var log in logs)
        {
            if (log.LocalDate == currentDate)
            {
                streak++;
            }
            else if (log.LocalDate == currentDate.AddDays(-1))
            {
                streak++;
                currentDate = log.LocalDate;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    public Task<decimal> GetTotalCaloriesAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserExerciseLogs
            .Where(l => l.UserId == userId)
            .SumAsync(l => l.TotalCalories, cancellationToken);
    }
}
