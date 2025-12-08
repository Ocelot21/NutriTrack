using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class MealRepository : EfRepository<Meal, MealId>, IMealRepository
{
    public MealRepository(AppDbContext dbContext) : base(dbContext) { }

    public Task<int> CountTotalItemsForUserAsync(
        UserId userId, 
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Meals
            .Where(m => m.UserId == userId)
            .SumAsync(m => m.Items.Count(), cancellationToken);
    }

    public async Task<IReadOnlyList<Meal>> GetByUserAndDateRangeAsync(
        UserId userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
        => await _dbContext.Meals
            .Where(m => m.UserId == userId && m.LocalDate >= from && m.LocalDate <= to)
            .ToListAsync(cancellationToken);

    public async Task<int> GetCurrentStreakDaysAsync(
        UserId userId,
        DateOnly localDate,
        CancellationToken cancellationToken = default)
    {
        var meals = await _dbContext.Meals
            .Where(m => m.UserId == userId && m.LocalDate <= localDate && m.Items.Any())
            .OrderByDescending(m => m.LocalDate)
            .ToListAsync(cancellationToken);

        if (meals.Count == 0)
        {
            return 0;
        }

        int streak = 1;

        for (int i = 1; i < meals.Count; i++)
        {
            if (meals[i].LocalDate == meals[i - 1].LocalDate.AddDays(-1))
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }
}