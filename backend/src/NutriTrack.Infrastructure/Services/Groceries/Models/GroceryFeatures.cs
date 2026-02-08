using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Services.Groceries.Models;

public sealed class GroceryFeatures
{
    public Guid GroceryId { get; set; }
    public float ProteinPer100 { get; set; }
    public float CarbsPer100 { get; set; }
    public float FatPer100 { get; set; }
    public float CaloriesPer100 { get; set; }
    public float PopularityScore { get; set; }
    public float SeasonalityScore { get; set; }
    public float CategoryScore { get; set; }
    public int GoalType { get; set; }
    public int UserAge { get; set; }
    public int UserGender { get; set; }
    public int UserActivityLevel { get; set; }

    public static GroceryFeatures FromGrocery(
        Grocery grocery,
        int popularityCount,
        NutritionGoal goalType,
        int userAge,
        int userGender,
        int userActivityLevel,
        float seasonalityScore)
    {
        return new GroceryFeatures
        {
            GroceryId = grocery.Id.Value,
            ProteinPer100 = (float)grocery.MacrosPer100.ProteinGramsPer100,
            CarbsPer100 = (float)grocery.MacrosPer100.CarbsGramsPer100,
            FatPer100 = (float)grocery.MacrosPer100.FatGramsPer100,
            CaloriesPer100 = grocery.CaloriesPer100,
            PopularityScore = MathF.Log10(1 + popularityCount),
            SeasonalityScore = seasonalityScore,
            CategoryScore = GetCategoryScore(grocery.Category, goalType),
            GoalType = (int)goalType,
            UserAge = userAge,
            UserGender = userGender,
            UserActivityLevel = userActivityLevel
        };
    }

    private static float GetCategoryScore(GroceryCategory category, NutritionGoal goalType)
    {
        return (category, goalType) switch
        {
            (GroceryCategory.Vegetable, NutritionGoal.LoseWeight) => 1.0f,
            (GroceryCategory.Fruit, NutritionGoal.LoseWeight) => 0.8f,
            (GroceryCategory.Protein, NutritionGoal.LoseWeight) => 0.7f,
            (GroceryCategory.Dairy, NutritionGoal.LoseWeight) => 0.5f,
            
            (GroceryCategory.Protein, NutritionGoal.GainWeight) => 1.0f,
            (GroceryCategory.Grain, NutritionGoal.GainWeight) => 0.9f,
            (GroceryCategory.Dairy, NutritionGoal.GainWeight) => 0.8f,
            (GroceryCategory.Snack, NutritionGoal.GainWeight) => 0.7f,
            
            (GroceryCategory.Vegetable, _) => 0.8f,
            (GroceryCategory.Fruit, _) => 0.7f,
            (GroceryCategory.Protein, _) => 0.7f,
            (GroceryCategory.Dairy, _) => 0.6f,
            _ => 0.5f
        };
    }
}

public sealed class GroceryPrediction
{
    public float Score { get; set; }
}
