using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Infrastructure.Services.Groceries.Models;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class GroceryRecommender : IGroceryRecommender, IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMealItemReadRepository _mealItemReadRepository;
    private readonly IMemoryCache _cache;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly MLRecommendationScorer _mlScorer;

    public GroceryRecommender(
        AppDbContext dbContext,
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IGroceryRepository groceryRepository,
        IMealItemReadRepository mealItemReadRepository,
        IMemoryCache cache,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _groceryRepository = groceryRepository;
        _mealItemReadRepository = mealItemReadRepository;
        _cache = cache;
        _dateTimeProvider = dateTimeProvider;
        _mlScorer = new MLRecommendationScorer();
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

        var currentGoal = await _userGoalRepository.GetCurrentForUser(userId);
        var goalType = currentGoal?.Type ?? user.NutritionGoal;

        const int maxCandidates = 1000;

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

        var cohortUserIds = await GetCohortUserIdsAsync(goalType, cancellationToken);
        var popularityByGroceryId = await _mealItemReadRepository.CountByGroceryForUsersAsync(
            cohortUserIds.Select(id => new UserId(id)).ToArray(),
            cancellationToken);

        var userAge = CalculateAge(user.Birthdate);
        var currentMonth = _dateTimeProvider.UtcNow.Month;

        var ranked = candidates
            .Select(g =>
            {
                var popularityCount = popularityByGroceryId.TryGetValue(g.Id.Value, out var cnt) ? cnt : 0;
                var seasonalityScore = SeasonalityCalculator.GetSeasonalityScore(g.Category, currentMonth);

                var features = GroceryFeatures.FromGrocery(
                    g,
                    popularityCount,
                    goalType,
                    userAge,
                    (int)user.Gender,
                    (int)user.ActivityLevel,
                    seasonalityScore);

                var score = _mlScorer.Score(features);

                var noveltyBoost = popularityCount < 5 ? 1.3 : 1.0;
                var coldStartBoost = GetColdStartBoost(user, popularityCount);

                return new
                {
                    Grocery = g,
                    Score = score * noveltyBoost * coldStartBoost
                };
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
        var cacheKey = $"cohort_users_{goalType}";
        
        if (_cache.TryGetValue<HashSet<Guid>>(cacheKey, out var cachedUserIds))
        {
            return cachedUserIds!;
        }

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

        _cache.Set(cacheKey, userIdsByProfileGoal, TimeSpan.FromHours(1));

        return userIdsByProfileGoal;
    }

    private static int CalculateAge(DateOnly? birthdate)
    {
        if (!birthdate.HasValue)
            return 30;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthdate.Value.Year;
        if (birthdate.Value > today.AddYears(-age))
            age--;

        return age;
    }

    private static double GetColdStartBoost(User user, int popularityCount)
    {
        if (!user.IsHealthProfileCompleted)
        {
            return popularityCount > 10 ? 1.2 : 1.0;
        }

        return 1.0;
    }

    public void Dispose()
    {
        _mlScorer.Dispose();
    }
}
