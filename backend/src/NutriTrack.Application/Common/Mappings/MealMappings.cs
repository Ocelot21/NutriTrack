using NutriTrack.Application.Meals.Common;
using NutriTrack.Domain.Meals;

namespace NutriTrack.Application.Common.Mappings;

public static class MealMappings
{
    public static MealResult ToMealResult(this Meal meal)
    {
        return new MealResult(
            meal.Id,
            meal.UserId,
            meal.Name,
            meal.Description,
            meal.OccurredAtUtc,
            meal.OccurredAtLocal,
            meal.LocalDate,
            meal.TotalCalories,
            meal.TotalProtein,
            meal.TotalCarbohydrates,
            meal.TotalFats);
    }
}
