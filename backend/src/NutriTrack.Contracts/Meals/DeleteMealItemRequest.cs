namespace NutriTrack.Contracts.Meals;

public sealed record DeleteMealItemRequest(
    Guid MealId,
    Guid MealItemId
);
