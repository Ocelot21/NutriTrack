using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.AdminDashboard;
using NutriTrack.Contracts.AdminDashboard;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Infrastructure.AdminDashboard;

public sealed class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _dbContext;

    public AdminDashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminDashboardSummaryResponse> GetSummaryAsync(
        GetAdminDashboardSummaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var days = request.Days <= 0 ? 7 : Math.Min(request.Days, 365);
        var top = request.Top <= 0 ? 10 : Math.Min(request.Top, 50);

        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddDays(-days);

        var mealsInRange = _dbContext.Meals
            .AsNoTracking()
            .Where(m => m.OccurredAtUtc >= fromUtc && m.OccurredAtUtc < toUtc);

        var exerciseLogsInRange = _dbContext.UserExerciseLogs
            .AsNoTracking()
            .Where(e => e.OccurredAtUtc >= fromUtc && e.OccurredAtUtc < toUtc);

        var mealsLogged = await mealsInRange.CountAsync(cancellationToken);
        var exerciseLogsLogged = await exerciseLogsInRange.CountAsync(cancellationToken);

        var activeUsers = await mealsInRange
            .Select(m => m.UserId)
            .Concat(exerciseLogsInRange.Select(e => e.UserId))
            .Distinct()
            .CountAsync(cancellationToken);

        var mealDailyCounts = await mealsInRange
            .GroupBy(m => m.LocalDate)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var exerciseDailyCounts = await exerciseLogsInRange
            .GroupBy(e => e.LocalDate)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var dailyCounts = Enumerable
            .Range(0, days)
            .Select(offset => DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-offset)))
            .Select(date => new AdminDashboardDailyCountResponse(
                Date: date,
                MealsLogged: mealDailyCounts.FirstOrDefault(x => x.Date == date)?.Count ?? 0,
                ExerciseLogsLogged: exerciseDailyCounts.FirstOrDefault(x => x.Date == date)?.Count ?? 0))
            .OrderBy(x => x.Date)
            .ToList();

        // Query underlying tables directly to avoid EF translation issues with owned collections (Meal.Items).
        var topGroceriesRows = await _dbContext.Database
            .SqlQuery<DashboardTopRow>($@"
                SELECT TOP({top}) g.Name AS [Name], COUNT(1) AS [Count]
                FROM MealItems mi
                INNER JOIN Meals m ON mi.MealId = m.Id
                INNER JOIN Groceries g ON mi.GroceryId = g.Id
                WHERE m.OccurredAtUtc >= {fromUtc} AND m.OccurredAtUtc < {toUtc}
                GROUP BY g.Name
                ORDER BY COUNT(1) DESC, g.Name ASC")
            .ToListAsync(cancellationToken);

        var topGroceries = topGroceriesRows
            .Select(r => new AdminDashboardTopItemResponse(r.Name, r.Count))
            .ToList();

        var topExercisesRows = await _dbContext.Database
            .SqlQuery<DashboardTopRow>($@"
                SELECT TOP({top}) e.Name AS [Name], COUNT(1) AS [Count]
                FROM UserExerciseLogs uel
                INNER JOIN Exercises e ON uel.ExerciseId = e.Id
                WHERE uel.OccurredAtUtc >= {fromUtc} AND uel.OccurredAtUtc < {toUtc}
                GROUP BY e.Name
                ORDER BY COUNT(1) DESC, e.Name ASC")
            .ToListAsync(cancellationToken);

        var topExercises = topExercisesRows
            .Select(r => new AdminDashboardTopItemResponse(r.Name, r.Count))
            .ToList();

        return new AdminDashboardSummaryResponse(
            Kpis: new AdminDashboardKpisResponse(
                ActiveUsers: activeUsers,
                MealsLogged: mealsLogged,
                ExerciseLogsLogged: exerciseLogsLogged),
            DailyCounts: dailyCounts,
            TopGroceries: topGroceries,
            TopExercises: topExercises);
    }

    private sealed record DashboardTopRow(string Name, int Count);
}
