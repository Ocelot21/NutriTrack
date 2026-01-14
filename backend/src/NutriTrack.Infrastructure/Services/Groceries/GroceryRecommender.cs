using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class GroceryRecommender : IGroceryRecommender
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMealItemReadRepository _mealItemReadRepository;

    public GroceryRecommender(
        AppDbContext dbContext,
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IGroceryRepository groceryRepository,
        IMealItemReadRepository mealItemReadRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _groceryRepository = groceryRepository;
        _mealItemReadRepository = mealItemReadRepository;
    }

    public async Task<PagedResult<GroceryResult>> GetRecommendedAsync(
        UserId userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return new PagedResult<GroceryResult>(new List<GroceryResult>(), 0, page, pageSize);
        }

        // Determine active goal type. Fallback to profile NutritionGoal if no goals.
        var currentGoal = await _userGoalRepository.GetCurrentForUser(userId);
        var goalType = currentGoal?.Type ?? user.NutritionGoal;

        // Candidate set: use existing repository logic to respect approvals and visibility.
        // We fetch a larger pool and rank it in-memory to keep consistent ordering.
        // For early version, cap candidates for performance.
        const int maxCandidates = 500;

        var candidatesPaged = await _groceryRepository.GetPagedAsync(
            filters: new GroceryListFilters(null, null, null, null, null, null, null, null, null, null, null),
            userId: userId,
            page: 1,
            pageSize: maxCandidates,
            cancellationToken: cancellationToken);

        var candidates = candidatesPaged.Items;
        if (candidates.Count == 0)
        {
            return new PagedResult<GroceryResult>(new List<GroceryResult>(), 0, page, pageSize);
        }

        // Build cohort popularity: groceries consumed by users who had/has same goal type.
        // Ignore users without completed health profile.
        var cohortUserIds = await GetCohortUserIdsAsync(goalType, cancellationToken);
        var popularityByGroceryId = await _mealItemReadRepository.CountByGroceryForUsersAsync(
            cohortUserIds.Select(id => new UserId(id)).ToArray(),
            cancellationToken);

        var ranked = candidates
            .Select(g => new
            {
                Grocery = g,
                Score = Score(g, goalType, popularityByGroceryId)
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Grocery.Name)
            .ToList();

        var total = ranked.Count;

        var pageItems = ranked
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Grocery.ToGroceryResult())
            .ToList();

        return new PagedResult<GroceryResult>(pageItems, total, page, pageSize);
    }

    private async Task<HashSet<Guid>> GetCohortUserIdsAsync(NutritionGoal goalType, CancellationToken ct)
    {
        // Users with completed health profile AND with an in-progress goal of this type,
        // plus users whose profile goal matches as fallback.
        var usersWithProfile = await _dbContext.Users
            .Where(u => u.IsHealthProfileCompleted)
            .Select(u => new { u.Id, u.NutritionGoal })
            .ToListAsync(ct);

        var userIdsByProfileGoal = usersWithProfile
            .Where(u => u.NutritionGoal == goalType)
            .Select(u => u.Id.Value)
            .ToHashSet();

        var userIdsByActiveGoal = await _dbContext.UserGoals
            .Where(g => g.Status == UserGoalStatus.InProgress && g.Type == goalType)
            .Select(g => g.UserId.Value)
            .ToListAsync(ct);

        foreach (var id in userIdsByActiveGoal)
        {
            userIdsByProfileGoal.Add(id);
        }

        return userIdsByProfileGoal;
    }

    private static double Score(Grocery g, NutritionGoal goalType, Dictionary<Guid, int> popularity)
    {
        var p = (double)g.MacrosPer100.ProteinGramsPer100;
        var c = (double)g.MacrosPer100.CarbsGramsPer100;
        var f = (double)g.MacrosPer100.FatGramsPer100;
        var kcal = (double)g.CaloriesPer100;

        var pop = popularity.TryGetValue(g.Id.Value, out var cnt) ? cnt : 0;
        var popScore = Math.Log10(1 + pop);

        // Simple goal-aware heuristic as an initial baseline.
        // Later you can replace this with ML.NET trained model output.
        var macroScore = goalType switch
        {
            NutritionGoal.LoseWeight => (p * 2.0) - (kcal * 0.02) - (f * 0.5) - (c * 0.2),
            NutritionGoal.GainWeight => (kcal * 0.02) + (c * 0.6) + (f * 0.8) + (p * 0.8),
            _ => (p * 1.0) + (c * 0.3) + (f * 0.3) - (Math.Abs(kcal - 200) * 0.01),
        };

        // Blend cohort popularity with macro score.
        return macroScore + (popScore * 1.5);
    }
}
