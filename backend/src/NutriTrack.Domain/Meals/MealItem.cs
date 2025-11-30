using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Groceries;

namespace NutriTrack.Domain.Meals;

public sealed class MealItem : Entity<MealItemId>
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private MealItem() : base()
    {
    }

    private MealItem(
        MealItemId id,
        MealId mealId,
        Grocery grocery,
        decimal quantity)
        : base(id)
    {
        MealId = mealId;
        GroceryId = grocery.Id;
        Snapshot = GrocerySnapshot.Create(
            grocery.Name,
            grocery.CaloriesPer100,
            grocery.MacrosPer100,
            grocery.UnitOfMeasure);

        Quantity = quantity;
    }

    public MealId MealId { get; private set; }

    public GroceryId GroceryId { get; private set; }
    public Grocery? Grocery { get; private set; } 

    public GrocerySnapshot Snapshot { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public static MealItem Create(
        MealItemId id,
        MealId mealId,
        Grocery grocery,
        decimal quantity)
    {
        if (mealId.Value == Guid.Empty)
        {
            throw new DomainException(
                DomainErrorCodes.MealItems.InvalidMeal,
                "Meal identifier is required.");
        }

        if (grocery is null)
        {
            throw new DomainException(
                DomainErrorCodes.MealItems.InvalidGrocery,
                "Grocery is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainException(
                DomainErrorCodes.MealItems.InvalidQuantity,
                "Quantity must be positive.");
        }

        return new MealItem(id, mealId, grocery, quantity);
    }

    public void ChangeQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException(
                DomainErrorCodes.MealItems.InvalidQuantity,
                "Quantity must be positive.");
        }

        Quantity = quantity;
    }
}
