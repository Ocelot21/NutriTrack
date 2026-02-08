using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Infrastructure.Persistence;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class RecommendationCacheWarmer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RecommendationCacheWarmer> _logger;
    private const int WarmupIntervalHours = 6;

    public RecommendationCacheWarmer(
        IServiceProvider serviceProvider,
        IMemoryCache cache,
        ILogger<RecommendationCacheWarmer> logger)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecommendationCacheWarmer starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await WarmupCacheAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(WarmupIntervalHours), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("RecommendationCacheWarmer stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache warmup");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task WarmupCacheAsync(CancellationToken ct)
    {
        _logger.LogInformation("Starting recommendation cache warmup");

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        foreach (var goalType in Enum.GetValues<NutritionGoal>())
        {
            var cacheKey = $"cohort_users_{goalType}";
            
            var usersWithProfile = await dbContext.Users
                .Where(u => u.IsHealthProfileCompleted)
                .Select(u => new { u.Id, u.NutritionGoal })
                .ToListAsync(ct);

            var userIdsByProfileGoal = usersWithProfile
                .Where(u => u.NutritionGoal == goalType)
                .Select(u => u.Id.Value)
                .ToHashSet();

            var userIdsByActiveGoal = await dbContext.UserGoals
                .Where(g => g.Status == UserGoalStatus.InProgress && g.Type == goalType)
                .Select(g => g.UserId.Value)
                .ToListAsync(ct);

            foreach (var id in userIdsByActiveGoal)
            {
                userIdsByProfileGoal.Add(id);
            }

            _cache.Set(cacheKey, userIdsByProfileGoal, TimeSpan.FromHours(WarmupIntervalHours + 1));
            
            _logger.LogInformation(
                "Cached {Count} cohort users for goal {GoalType}",
                userIdsByProfileGoal.Count,
                goalType);
        }

        _logger.LogInformation("Recommendation cache warmup completed");
    }
}
