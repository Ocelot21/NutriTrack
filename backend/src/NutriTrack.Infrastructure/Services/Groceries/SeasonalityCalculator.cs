using NutriTrack.Domain.Groceries;

namespace NutriTrack.Infrastructure.Services.Groceries;

public static class SeasonalityCalculator
{
    public static float GetSeasonalityScore(GroceryCategory category, int currentMonth)
    {
        return category switch
        {
            GroceryCategory.Vegetable => GetVegetableSeasonality(currentMonth),
            GroceryCategory.Fruit => GetFruitSeasonality(currentMonth),
            _ => 1.0f
        };
    }

    private static float GetVegetableSeasonality(int month)
    {
        return month switch
        {
            >= 3 and <= 5 => 1.2f,
            >= 6 and <= 8 => 1.3f,
            >= 9 and <= 11 => 1.1f,
            _ => 0.9f
        };
    }

    private static float GetFruitSeasonality(int month)
    {
        return month switch
        {
            >= 6 and <= 9 => 1.3f,
            >= 10 and <= 11 => 1.1f,
            >= 3 and <= 5 => 1.0f,
            _ => 0.8f
        };
    }
}
