using NutriTrack.Domain.Groceries;

namespace NutriTrack.Infrastructure.Services.Groceries;

public sealed class DiversityManager
{
    private readonly Dictionary<Guid, DateTime> _recentlyRecommended = new();
    private readonly Dictionary<GroceryCategory, int> _categoryCount = new();
    private const int MaxRecentItems = 50;

    public void RecordRecommendation(Guid groceryId, GroceryCategory category)
    {
        _recentlyRecommended[groceryId] = DateTime.UtcNow;
        
        if (!_categoryCount.ContainsKey(category))
            _categoryCount[category] = 0;
        
        _categoryCount[category]++;

        if (_recentlyRecommended.Count > MaxRecentItems)
        {
            var oldest = _recentlyRecommended.OrderBy(x => x.Value).First();
            _recentlyRecommended.Remove(oldest.Key);
        }
    }

    public float GetDiversityPenalty(Guid groceryId, GroceryCategory category)
    {
        float penalty = 1.0f;

        if (_recentlyRecommended.TryGetValue(groceryId, out var lastSeen))
        {
            var hoursSince = (DateTime.UtcNow - lastSeen).TotalHours;
            if (hoursSince < 24)
                penalty *= 0.3f;
            else if (hoursSince < 72)
                penalty *= 0.6f;
            else if (hoursSince < 168)
                penalty *= 0.8f;
        }

        if (_categoryCount.TryGetValue(category, out var count))
        {
            var totalRecommendations = _categoryCount.Values.Sum();
            if (totalRecommendations > 0)
            {
                var categoryRatio = (float)count / totalRecommendations;
                if (categoryRatio > 0.4f)
                    penalty *= 0.7f;
                else if (categoryRatio > 0.3f)
                    penalty *= 0.85f;
            }
        }

        return penalty;
    }

    public void Reset()
    {
        _recentlyRecommended.Clear();
        _categoryCount.Clear();
    }
}
