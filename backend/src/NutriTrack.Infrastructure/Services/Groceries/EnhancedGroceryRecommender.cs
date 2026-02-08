using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Infrastructure.Services.Groceries.Models;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class EnhancedGroceryRecommender : IEnhancedGroceryRecommender, IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMealItemReadRepository _mealItemReadRepository;
    private readonly IMemoryCache _cache;
    private readonly IRecommendationMetricsTracker _metricsTracker;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly MLRecommendationScorer _mlScorer;
    private readonly DiversityManager _diversityManager;

    private const int MaxCandidates = 1000;
    private const double NoveltyBoostThreshold = 5;
    private const double NoveltyBoostMultiplier = 1.3;

    public EnhancedGroceryRecommender(
        AppDbContext dbContext,
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IGroceryRepository groceryRepository,
        IMealItemReadRepository mealItemReadRepository,
        IMemoryCache cache,
        IRecommendationMetricsTracker metricsTracker,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _groceryRepository = groceryRepository;
        _mealItemReadRepository = mealItemReadRepository;
        _cache = cache;
        _metricsTracker = metricsTracker;
        _dateTimeProvider = dateTimeProvider;
        _mlScorer = new MLRecommendationScorer();
        _diversityManager = new DiversityManager();
    }

    public async Task<PagedResult<GroceryRecommendationResult>> GetRecommendedAsync(
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
            return new PagedResult<GroceryRecommendationResult>(
                new List<GroceryRecommendationResult>(), 0, page, pageSize);
        }

        var currentGoal = await _userGoalRepository.GetCurrentForUser(userId);
        var goalType = currentGoal?.Type ?? user.NutritionGoal;

        var candidates = await GetCandidatesAsync(userId, cancellationToken);
        if (candidates.Count == 0)
        {
            return new PagedResult<GroceryRecommendationResult>(
                new List<GroceryRecommendationResult>(), 0, page, pageSize);
        }

        var cohortUserIds = await GetCohortUserIdsAsync(goalType, cancellationToken);
        var popularityByGroceryId = await _mealItemReadRepository.CountByGroceryForUsersAsync(
            cohortUserIds.Select(id => new UserId(id)).ToArray(),
            cancellationToken);

        var userAge = CalculateAge(user.Birthdate);
        var currentMonth = _dateTimeProvider.UtcNow.Month;

        var userRecentGroceries = await GetUserRecentGroceriesAsync(userId, cancellationToken);

        var ranked = candidates
            .Select(grocery =>
            {
                var popularityCount = popularityByGroceryId.TryGetValue(grocery.Id.Value, out var cnt) ? cnt : 0;
                var seasonalityScore = SeasonalityCalculator.GetSeasonalityScore(grocery.Category, currentMonth);

                var features = GroceryFeatures.FromGrocery(
                    grocery,
                    popularityCount,
                    goalType,
                    userAge,
                    (int)user.Gender,
                    (int)user.ActivityLevel,
                    seasonalityScore);

                var baseScore = _mlScorer.Score(features);

                var noveltyBoost = popularityCount < NoveltyBoostThreshold ? NoveltyBoostMultiplier : 1.0;
                var diversityPenalty = _diversityManager.GetDiversityPenalty(grocery.Id.Value, grocery.Category);
                var coldStartBoost = GetColdStartBoost(user, popularityCount);

                var finalScore = baseScore * noveltyBoost * diversityPenalty * coldStartBoost;

                var explanation = BuildExplanation(
                    grocery,
                    goalType,
                    popularityCount,
                    seasonalityScore,
                    userRecentGroceries.Contains(grocery.Id.Value));

                return new
                {
                    Grocery = grocery,
                    Score = finalScore,
                    Explanation = explanation
                };
            })
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Grocery.Name)
            .ToList();

        for (int i = 0; i < Math.Min(50, ranked.Count); i++)
        {
            _diversityManager.RecordRecommendation(
                ranked[i].Grocery.Id.Value,
                ranked[i].Grocery.Category);
        }

        var total = ranked.Count;

        var pageItems = ranked
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select((x, index) => new GroceryRecommendationResult(
                x.Grocery.Id,
                x.Grocery.Name,
                x.Grocery.Category,
                x.Grocery.Barcode,
                x.Grocery.MacrosPer100,
                x.Grocery.CaloriesPer100,
                x.Grocery.UnitOfMeasure,
                x.Grocery.GramsPerPiece,
                x.Grocery.ImageUrl,
                x.Grocery.IsApproved,
                x.Grocery.IsDeleted,
                x.Score,
                x.Explanation))
            .ToList();

        for (int i = 0; i < pageItems.Count; i++)
        {
            var position = ((page - 1) * pageSize) + i + 1;
            await _metricsTracker.TrackRecommendationShownAsync(
                userId.Value,
                pageItems[i].Id.Value,
                pageItems[i].Score,
                position,
                cancellationToken);
        }

        return new PagedResult<GroceryRecommendationResult>(pageItems, total, page, pageSize);
    }

    private async Task<List<Grocery>> GetCandidatesAsync(UserId userId, CancellationToken ct)
    {
        var candidatesPaged = await _groceryRepository.GetPagedAsync(
            filters: new GroceryListFilters(null, null, null, null, null, null, null, null, null, null, null),
            userId: userId,
            page: 1,
            pageSize: MaxCandidates,
            cancellationToken: ct);

        return candidatesPaged.Items.ToList();
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

    private async Task<HashSet<Guid>> GetUserRecentGroceriesAsync(UserId userId, CancellationToken ct)
    {
        var thirtyDaysAgo = _dateTimeProvider.UtcNow.AddDays(-30);
        
        var recentGroceryIds = await _dbContext.Set<Domain.Meals.Meal>()
            .Where(m => m.UserId == userId && m.OccurredAtUtc >= thirtyDaysAgo)
            .SelectMany(m => m.Items)
            .Select(item => item.GroceryId.Value)
            .Distinct()
            .ToListAsync(ct);

        return recentGroceryIds.ToHashSet();
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

    private static string BuildExplanation(
        Grocery grocery,
        NutritionGoal goalType,
        int popularityCount,
        float seasonalityScore,
        bool recentlyConsumed)
    {
        var reasons = new List<string>();

        if (popularityCount > 50)
            reasons.Add($"Popular among users with similar goals ({popularityCount} users)");
        else if (popularityCount > 20)
            reasons.Add("Well-liked by your cohort");
        else if (popularityCount < 5)
            reasons.Add("New suggestion to try");

        if (seasonalityScore > 1.1f)
            reasons.Add("In season now");

        if (goalType == NutritionGoal.LoseWeight)
        {
            if (grocery.MacrosPer100.ProteinGramsPer100 > 15)
                reasons.Add("High in protein");
            if (grocery.CaloriesPer100 < 100)
                reasons.Add("Low calorie option");
        }
        else if (goalType == NutritionGoal.GainWeight)
        {
            if (grocery.CaloriesPer100 > 300)
                reasons.Add("High calorie for muscle gain");
            if (grocery.MacrosPer100.CarbsGramsPer100 > 40)
                reasons.Add("Rich in carbohydrates");
        }

        if (grocery.Category == GroceryCategory.Vegetable || grocery.Category == GroceryCategory.Fruit)
            reasons.Add("Nutrient-dense whole food");

        if (recentlyConsumed)
            reasons.Add("You've enjoyed this recently");

        return reasons.Count > 0 
            ? string.Join(", ", reasons) 
            : "Recommended for your nutrition plan";
    }

    public void Dispose()
    {
        _mlScorer.Dispose();
    }
}
