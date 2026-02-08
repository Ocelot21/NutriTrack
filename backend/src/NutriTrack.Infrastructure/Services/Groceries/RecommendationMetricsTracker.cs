using Microsoft.Extensions.Logging;
using NutriTrack.Application.Common.Interfaces.Services;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class RecommendationMetricsTracker : IRecommendationMetricsTracker
{
    private readonly ILogger<RecommendationMetricsTracker> _logger;

    public RecommendationMetricsTracker(ILogger<RecommendationMetricsTracker> logger)
    {
        _logger = logger;
    }

    public Task TrackRecommendationShownAsync(
        Guid userId,
        Guid groceryId,
        double score,
        int position,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recommendation shown: UserId={UserId}, GroceryId={GroceryId}, Score={Score:F2}, Position={Position}",
            userId, groceryId, score, position);
        
        return Task.CompletedTask;
    }

    public Task TrackRecommendationClickedAsync(
        Guid userId,
        Guid groceryId,
        int position,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recommendation clicked: UserId={UserId}, GroceryId={GroceryId}, Position={Position}",
            userId, groceryId, position);
        
        return Task.CompletedTask;
    }

    public Task TrackRecommendationConvertedAsync(
        Guid userId,
        Guid groceryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Recommendation converted: UserId={UserId}, GroceryId={GroceryId}",
            userId, groceryId);
        
        return Task.CompletedTask;
    }
}
