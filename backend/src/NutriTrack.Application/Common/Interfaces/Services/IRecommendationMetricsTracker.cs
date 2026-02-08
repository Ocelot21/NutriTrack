namespace NutriTrack.Application.Common.Interfaces.Services;

public interface IRecommendationMetricsTracker
{
    Task TrackRecommendationShownAsync(
        Guid userId,
        Guid groceryId,
        double score,
        int position,
        CancellationToken cancellationToken = default);

    Task TrackRecommendationClickedAsync(
        Guid userId,
        Guid groceryId,
        int position,
        CancellationToken cancellationToken = default);

    Task TrackRecommendationConvertedAsync(
        Guid userId,
        Guid groceryId,
        CancellationToken cancellationToken = default);
}
