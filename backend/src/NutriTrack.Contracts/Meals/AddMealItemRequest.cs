namespace NutriTrack.Contracts.Meals;

public sealed record AddMealItemRequest(
    Guid MealId,
    Guid GroceryId,
    decimal Quantity
);
