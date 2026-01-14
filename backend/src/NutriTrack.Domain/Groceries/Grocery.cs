using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Groceries.Events;
using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Groceries;

public sealed class Grocery : AggregateRoot<GroceryId>
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private Grocery() : base()
    {
        // Parameterless constructor required by EF Core
    }

    private Grocery(
        GroceryId id,
        string name,
        GroceryCategory category,
        MacroNutrients macros,
        int caloriesPer100g,
        string? barcode,
        UnitOfMeasure unitOfMeasure,
        decimal? gramsPerPiece,
        string? imageUrl,
        bool isApproved)
        : base(id)
    {
        Name = name;
        Category = category;
        MacrosPer100 = macros;
        CaloriesPer100 = caloriesPer100g;
        Barcode = barcode;
        UnitOfMeasure = unitOfMeasure;
        GramsPerPiece = gramsPerPiece;
        ImageUrl = imageUrl;
        IsApproved = isApproved;
    }

    public string Name { get; private set; } = null!;
    public GroceryCategory Category { get; private set; }
    public string? Barcode { get; private set; }

    public MacroNutrients MacrosPer100 { get; private set; } = null!; 
    public int CaloriesPer100 { get; private set; }
    public UnitOfMeasure UnitOfMeasure { get; private set; }
    public decimal? GramsPerPiece { get; private set; }
    public string? ImageUrl { get; private set; }

    public bool IsApproved { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    // --------- Factory ---------

    public static Grocery Create(
        string name,
        GroceryCategory category,
        MacroNutrients macros,
        int caloriesPer100,
        UnitOfMeasure unitOfMeasure,
        decimal? gramsPerPiece = null,
        string? barcode = null,
        string? imageUrl = null,
        bool isApproved = true)
    {
        name = NormalizeName(name);
        category = NormalizeCategory(category);
        macros = macros ?? throw new DomainException(DomainErrorCodes.Groceries.InvalidMacros, "Macros cannot be null.");
        caloriesPer100 = NormalizeCalories(caloriesPer100);
        unitOfMeasure = ValidateUnitOfMeasure(unitOfMeasure, gramsPerPiece);

        string? normalizedBarcode = null;
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            normalizedBarcode = NormalizeBarcode(barcode);
        }

        return new Grocery(
            new GroceryId(Guid.NewGuid()),
            name,
            category,
            macros,
            caloriesPer100,
            normalizedBarcode,
            unitOfMeasure,
            gramsPerPiece,
            imageUrl,
            isApproved);
    }

    // --------- Domain methods ---------

    public void Approve()
    {
        if (IsApproved)
            return;

        IsApproved = true;

        if (CreatedBy is not null)
        {
            RaiseDomainEvent(new GrocerySuggestionApprovedDomainEvent(
                Id,
                CreatedBy.Value));
        }
    }

    public void Update(
        Optional<string> name,
        Optional<GroceryCategory> category,
        Optional<MacroNutrients> macros,
        Optional<int> caloriesPer100g,
        Optional<UnitOfMeasure> unitOfMeasure,
        Optional<decimal?> gramsPerPiece,
        Optional<string> barcode
        )
    {
        if (name.IsSet)
        {
            Name = NormalizeName(name.Value);
        }

        if (category.IsSet)
        {
            Category = NormalizeCategory(category.Value);
        }

        if (macros.IsSet)
        {
            MacrosPer100 = macros.Value ?? throw new DomainException(DomainErrorCodes.Groceries.InvalidMacros, "Macros cannot be null.");
        }

        if (caloriesPer100g.IsSet)
        {
            CaloriesPer100 = NormalizeCalories(caloriesPer100g.Value);
        }

        if (barcode.IsSet)
        {
            Barcode = NormalizeBarcode(barcode.Value);
        }

        if (unitOfMeasure.IsSet)
        {
            decimal? gramsPerPieceValue = gramsPerPiece.IsSet ? gramsPerPiece.Value : null;
            UnitOfMeasure = ValidateUnitOfMeasure(unitOfMeasure.Value, gramsPerPieceValue);
        }

        if (gramsPerPiece.IsSet)
        {
            GramsPerPiece = gramsPerPiece.Value;
        }
    }

    public void SetImage(string? imageBlobName)
    {
        ImageUrl = string.IsNullOrWhiteSpace(imageBlobName) ? null : imageBlobName;
    }

    // --------- Private helpers ---------

    private static string NormalizeName(string value)
    {
        value = value.Trim();

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

    private static GroceryCategory NormalizeCategory(GroceryCategory category)
    {
        if (!Enum.IsDefined(category))
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidCategory,
                "Invalid grocery category.");
        }

        return category;
    }

    private static int NormalizeCalories(int caloriesPer100g)
    {
        if (caloriesPer100g < 0 || caloriesPer100g >= DomainConstraints.Groceries.MaxCaloriesPer100)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidCalories,
                $"Calories per 100g must be between 0 and less than {DomainConstraints.Groceries.MaxCaloriesPer100}.");
        }

        return caloriesPer100g;
    }

    private static string? NormalizeBarcode(string barcode)
    {
        var value = barcode.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Length > DomainConstraints.Groceries.MaxBarcodeLength)
        {
            throw new DomainException(
                DomainErrorCodes.Groceries.InvalidBarcode,
                $"Barcode cannot be longer than {DomainConstraints.Groceries.MaxBarcodeLength} characters.");
        }

        return value;
    }

    private static UnitOfMeasure ValidateUnitOfMeasure(UnitOfMeasure unitOfMeasure, decimal? gramsPerPiece)
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