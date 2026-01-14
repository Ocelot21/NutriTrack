using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Meals;

public sealed class GrocerySnapshot : ValueObject
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private GrocerySnapshot()
    {
    }

    private GrocerySnapshot(
        string groceryName,
        int caloriesPer100,
        MacroNutrients macrosPer100,
        UnitOfMeasure unitOfMeasure,
        decimal? gramsPerPiece)
    {
        GroceryName = NormalizeName(groceryName);
        CaloriesPer100 = ValidateCalories(caloriesPer100);
        MacrosPer100 = macrosPer100;
        UnitOfMeasure = ValidateUnitOfMeasure(unitOfMeasure, gramsPerPiece);
        GramsPerPiece = gramsPerPiece;
    }

    public string GroceryName { get; private set; } = null!;
    public int CaloriesPer100 { get; private set; }
    public MacroNutrients MacrosPer100 { get; private set; } = null!;
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public decimal? GramsPerPiece { get; private set; }

    public static GrocerySnapshot Create(
        string groceryName,
        int caloriesPer100,
        MacroNutrients macrosPer100,
        UnitOfMeasure unitOfMeasure,
        decimal? gramsPerPiece = null)
    {
        macrosPer100 ??= new MacroNutrients(0m, 0m, 0m);
        return new GrocerySnapshot(groceryName, caloriesPer100, macrosPer100, unitOfMeasure, gramsPerPiece);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return GroceryName;
        yield return CaloriesPer100;
        yield return MacrosPer100;
        yield return UnitOfMeasure;
    }

    //------------------- Private helpers ------------------------------

    private static string NormalizeName(string name)
    {
        var value = name.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidName,
                "Grocery name cannot be empty.");
        }

        if (value.Length > DomainConstraints.Groceries.MaxNameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidName,
                $"Grocery name cannot be longer than {DomainConstraints.Groceries.MaxNameLength} characters.");
        }

        return value;
    }

    private static int ValidateCalories(int caloriesPer100)
    {
        if (caloriesPer100 < 0 || caloriesPer100 >= DomainConstraints.Groceries.MaxCaloriesPer100)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidCalories,
                $"Calories per 100 units must be between 0 and less than {DomainConstraints.Groceries.MaxCaloriesPer100}.");
        }

        return caloriesPer100;
    }

    private static UnitOfMeasure ValidateUnitOfMeasure(
        UnitOfMeasure unitOfMeasure,
        decimal? gramsPerPiece)
    {
        if (!Enum.IsDefined(unitOfMeasure))
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidUnitOfMeasure,
                "Unit of measure is invalid.");
        }

        if (unitOfMeasure == UnitOfMeasure.Piece)
        {
            if (!gramsPerPiece.HasValue)
            {
                throw new DomainException(
                    DomainErrorCodes.Groceries.GramsPerPieceNotSet,
                    "Grams per piece must be set when unit of measure is 'Piece'.");
            }
        }

        return unitOfMeasure;
    }
}
