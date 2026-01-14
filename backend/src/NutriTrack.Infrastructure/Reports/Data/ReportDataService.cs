using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Application.Reports.Services;
using NutriTrack.Domain.Reports;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Infrastructure.Reports.Data;

public sealed class ReportDataService : IReportDataService
{
    private readonly AppDbContext _dbContext;

    public ReportDataService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WeeklyOverviewReportData> GetWeeklyOverviewAsync(
        ReportRun run,
        CancellationToken cancellationToken = default)
    {
        var fromUtc = run.FromUtc.UtcDateTime;
        var toUtc = run.ToUtc.UtcDateTime;


        var userId = run.RequestedBy;

        var mealsQuery = _dbContext.Meals
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.OccurredAtUtc >= fromUtc && m.OccurredAtUtc < toUtc);

        var mealsCount = await mealsQuery.CountAsync(cancellationToken);

        var mealAgg = await mealsQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalCalories = g.Sum(x => x.TotalCalories),
                Protein = g.Sum(x => x.TotalProtein),
                Carbs = g.Sum(x => x.TotalCarbohydrates),
                Fat = g.Sum(x => x.TotalFats)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var exercisesQuery = _dbContext.UserExerciseLogs
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.OccurredAtUtc >= fromUtc && e.OccurredAtUtc < toUtc);

        var exerciseCount = await exercisesQuery.CountAsync(cancellationToken);

        var exerciseAgg = await exercisesQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Minutes = g.Sum(x => x.DurationMinutes),
                Calories = g.Sum(x => x.TotalCalories)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new WeeklyOverviewReportData(
            MealsCount: mealsCount,
            TotalCaloriesConsumed: mealAgg?.TotalCalories ?? 0,
            TotalProteinGrams: mealAgg?.Protein ?? 0,
            TotalCarbsGrams: mealAgg?.Carbs ?? 0,
            TotalFatGrams: mealAgg?.Fat ?? 0,
            ExerciseCount: exerciseCount,
            TotalMinutesExercised: exerciseAgg?.Minutes ?? 0,
            TotalCaloriesBurned: exerciseAgg?.Calories ?? 0);
    }

    public async Task<UserActivityReportData> GetUserActivityAsync(
        ReportRun run,
        CancellationToken cancellationToken = default)
    {
        var fromUtc = run.FromUtc.UtcDateTime;
        var toUtc = run.ToUtc.UtcDateTime;
        var userId = run.RequestedBy;

        var mealsQuery = _dbContext.Meals
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.OccurredAtUtc >= fromUtc && m.OccurredAtUtc < toUtc);

        var mealsCount = await mealsQuery.CountAsync(cancellationToken);

        var mealAgg = await mealsQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalCalories = g.Sum(x => x.TotalCalories),
                Protein = g.Sum(x => x.TotalProtein),
                Carbs = g.Sum(x => x.TotalCarbohydrates),
                Fat = g.Sum(x => x.TotalFats)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var exercisesQuery = _dbContext.UserExerciseLogs
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.OccurredAtUtc >= fromUtc && e.OccurredAtUtc < toUtc);

        var exerciseCount = await exercisesQuery.CountAsync(cancellationToken);

        var exerciseAgg = await exercisesQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Minutes = g.Sum(x => x.DurationMinutes),
                Calories = g.Sum(x => x.TotalCalories)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new UserActivityReportData(
            MealsCount: mealsCount,
            TotalCaloriesConsumed: mealAgg?.TotalCalories ?? 0,
            TotalProteinGrams: mealAgg?.Protein ?? 0,
            TotalCarbsGrams: mealAgg?.Carbs ?? 0,
            TotalFatGrams: mealAgg?.Fat ?? 0,
            ExerciseCount: exerciseCount,
            TotalMinutesExercised: exerciseAgg?.Minutes ?? 0,
            TotalCaloriesBurned: exerciseAgg?.Calories ?? 0);
    }

    public async Task<AdminAuditReportData> GetAdminAuditAsync(
        ReportRun run,
        CancellationToken cancellationToken = default)
    {
        var fromUtc = run.FromUtc.UtcDateTime;
        var toUtc = run.ToUtc.UtcDateTime;


        async Task<int> CreatedTotalAsync<TEntity>(IQueryable<TEntity> set)
            where TEntity : class
        {
            return await set
                .AsNoTracking()
                .CountAsync(
                    x => EF.Property<DateTime>(x, "CreatedAtUtc") >= fromUtc && EF.Property<DateTime>(x, "CreatedAtUtc") < toUtc,
                    cancellationToken);
        }

        async Task<int> ModifiedTotalAsync<TEntity>(IQueryable<TEntity> set)
            where TEntity : class
        {
            return await set
                .AsNoTracking()
                .CountAsync(
                    x => EF.Property<DateTime?>(x, "ModifiedAtUtc") >= fromUtc && EF.Property<DateTime?>(x, "ModifiedAtUtc") < toUtc,
                    cancellationToken);
        }

        async Task<List<AuditCountRow>> CreatedCountsByUserAsync<TEntity>(IQueryable<TEntity> set)
            where TEntity : class
        {
            return await set
                .AsNoTracking()
                .Where(x => EF.Property<DateTime>(x, "CreatedAtUtc") >= fromUtc && EF.Property<DateTime>(x, "CreatedAtUtc") < toUtc)
                .Select(x => EF.Property<NutriTrack.Domain.Users.UserId?>(x, "CreatedBy"))
                .Where(x => x != null)
                .GroupBy(x => x)
                .Select(g => new AuditCountRow(UserId: g.Key, Count: g.Count()))
                .ToListAsync(cancellationToken);
        }

        async Task<List<AuditCountRow>> ModifiedCountsByUserAsync<TEntity>(IQueryable<TEntity> set)
            where TEntity : class
        {
            return await set
                .AsNoTracking()
                .Where(x => EF.Property<DateTime?>(x, "ModifiedAtUtc") >= fromUtc && EF.Property<DateTime?>(x, "ModifiedAtUtc") < toUtc)
                .Select(x => EF.Property<NutriTrack.Domain.Users.UserId?>(x, "ModifiedBy"))
                .Where(x => x != null)
                .GroupBy(x => x)
                .Select(g => new AuditCountRow(UserId: g.Key, Count: g.Count()))
                .ToListAsync(cancellationToken);
        }

        var entities = new List<EntityAuditRow>
        {
            new(
                Entity: "Users",
                CreatedCount: await CreatedTotalAsync(_dbContext.Users),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Users)),
            new(
                Entity: "Roles",
                CreatedCount: await CreatedTotalAsync(_dbContext.Roles),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Roles)),
            new(
                Entity: "Permissions",
                CreatedCount: await CreatedTotalAsync(_dbContext.Permissions),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Permissions)),
            new(
                Entity: "Groceries",
                CreatedCount: await CreatedTotalAsync(_dbContext.Groceries),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Groceries)),
            new(
                Entity: "Meals",
                CreatedCount: await CreatedTotalAsync(_dbContext.Meals),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Meals)),
            new(
                Entity: "Exercises",
                CreatedCount: await CreatedTotalAsync(_dbContext.Exercises),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.Exercises)),
            new(
                Entity: "UserExerciseLogs",
                CreatedCount: await CreatedTotalAsync(_dbContext.UserExerciseLogs),
                ModifiedCount: await ModifiedTotalAsync(_dbContext.UserExerciseLogs))
        };

        static IReadOnlyList<AuditCountRow> MergeByUser(IEnumerable<IEnumerable<AuditCountRow>> parts) => parts
            .SelectMany(x => x)
            .GroupBy(x => x.UserId)
            .Select(g => new AuditCountRow(g.Key, g.Sum(x => x.Count)))
            .OrderByDescending(x => x.Count)
            .ToList();

        var createdByUser = MergeByUser(new[]
        {
            await CreatedCountsByUserAsync(_dbContext.Users),
            await CreatedCountsByUserAsync(_dbContext.Roles),
            await CreatedCountsByUserAsync(_dbContext.Permissions),
            await CreatedCountsByUserAsync(_dbContext.Groceries),
            await CreatedCountsByUserAsync(_dbContext.Meals),
            await CreatedCountsByUserAsync(_dbContext.Exercises),
            await CreatedCountsByUserAsync(_dbContext.UserExerciseLogs)
        });

        var modifiedByUser = MergeByUser(new[]
        {
            await ModifiedCountsByUserAsync(_dbContext.Users),
            await ModifiedCountsByUserAsync(_dbContext.Roles),
            await ModifiedCountsByUserAsync(_dbContext.Permissions),
            await ModifiedCountsByUserAsync(_dbContext.Groceries),
            await ModifiedCountsByUserAsync(_dbContext.Meals),
            await ModifiedCountsByUserAsync(_dbContext.Exercises),
            await ModifiedCountsByUserAsync(_dbContext.UserExerciseLogs)
        });

        entities = entities
            .OrderByDescending(x => x.CreatedCount + x.ModifiedCount)
            .ToList();

        return new AdminAuditReportData(
            Entities: entities,
            CreatedByUser: createdByUser,
            ModifiedByUser: modifiedByUser);
    }
}
