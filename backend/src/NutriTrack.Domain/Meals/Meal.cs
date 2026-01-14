using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Meals;

public sealed class Meal : AggregateRoot<MealId>
{
    private readonly List<MealItem> _items = new();

    private Meal() : base() { }

    private Meal(
        MealId id,
        UserId userId,
        string name,
        string? description,
        DateTime occurredAtUtc,
        DateTimeOffset occurredAtLocal,
        DateOnly localDate)
        : base(id)
    {
        UserId = userId;
        Name = name;
        Description = description;
        OccurredAtUtc = occurredAtUtc;
        OccurredAtLocal = occurredAtLocal;
        LocalDate = localDate;

        RecomputeTotals();
    }

    public UserId UserId { get; private set; }
    public User? User { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }
    public DateTimeOffset OccurredAtLocal { get; private set; }
    public DateOnly LocalDate { get; private set; }

    public int TotalCalories { get; private set; }
    public decimal TotalProtein { get; private set; }
    public decimal TotalCarbohydrates { get; private set; }
    public decimal TotalFats { get; private set; }

    public IReadOnlyCollection<MealItem> Items => _items.AsReadOnly();

    public static Meal Create(
        UserId userId,
        string name,
        DateTime occurredAtUtc,
        DateTimeOffset occurredAtLocal,
        DateOnly localDate,
        string? description = null)
    {
        if (userId.Value == Guid.Empty)
        {
            throw new DomainException(DomainErrorCodes.Meals.InvalidUser, "User identifier is required.");
        }

        name = NormalizeName(name);
        description = NormalizeDescription(description);
        occurredAtUtc = EnsureUtc(occurredAtUtc);
        occurredAtLocal = NormalizeLocal(occurredAtLocal);
        localDate = NormalizeLocalDate(localDate);

        return new Meal(
            new MealId(Guid.NewGuid()),
            userId,
            name,
            description,
            occurredAtUtc,
            occurredAtLocal,
            localDate);
    }

    public void UpdateDetails(
        Optional<string> name,
        Optional<string?> description,
        Optional<DateTime> occurredAtUtc,
        Optional<DateTimeOffset> occurredAtLocal,
        Optional<DateOnly> localDate)
    {
        if (name.IsSet)
        {
            Name = NormalizeName(name.Value);
        }

        if (description.IsSet)
        {
            Description = NormalizeDescription(description.Value);
        }

        if (occurredAtUtc.IsSet)
        {
            OccurredAtUtc = EnsureUtc(occurredAtUtc.Value);
        }

        if (occurredAtLocal.IsSet)
        {
            OccurredAtLocal = NormalizeLocal(occurredAtLocal.Value);
        }

        if (localDate.IsSet)
        {
            LocalDate = NormalizeLocalDate(localDate.Value);
        }
    }

    public MealItem AddItem(Grocery grocery, decimal quantity)
    {
        if (grocery is null)
        {
            throw new DomainException(DomainErrorCodes.MealItems.InvalidGrocery, "Grocery must be provided when adding a meal item.");
        }

        if (quantity <= 0)
        {
            throw new DomainException(DomainErrorCodes.MealItems.InvalidQuantity, "Quantity must be positive.");
        }

        var item = MealItem.Create(new MealItemId(Guid.NewGuid()), Id, grocery, quantity);

        _items.Add(item);
        RecomputeTotals();

        return item;
    }

    public void UpdateItem(MealItemId itemId, decimal quantity)
    {
        var item = _items.SingleOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            throw new DomainException(DomainErrorCodes.Meals.ItemNotFound, "Meal item does not exist.");
        }

        item.ChangeQuantity(quantity);
        RecomputeTotals();
    }

    public void RemoveItem(MealItemId itemId)
    {
        var item = _items.SingleOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            throw new DomainException(DomainErrorCodes.Meals.ItemNotFound, "Meal item does not exist.");
        }

        _items.Remove(item);
        RecomputeTotals();
    }

    private void RecomputeTotals()
    {
        decimal kcal = 0;
        decimal p = 0;
        decimal c = 0;
        decimal f = 0;

        foreach (var item in _items)
        {
            decimal factor;

            switch (item.Snapshot.UnitOfMeasure)
            {
                case UnitOfMeasure.Gram:
                case UnitOfMeasure.Milliliter:
                default:
                { 
                    factor = item.Quantity / 100m;
                    break;
                }

                case UnitOfMeasure.Piece:
                {
                    if (item.Snapshot.GramsPerPiece is null)
                    {
                        throw new DomainException(
                            DomainErrorCodes.Groceries.GramsPerPieceNotSet,
                            "Grams per piece must be set when unit of measure is 'Piece'.");
                        }

                    var totalGrams = item.Quantity * item.Snapshot.GramsPerPiece.Value;
                    factor = totalGrams / 100m;
                    break;
                }
            }

            kcal += factor * item.Snapshot.CaloriesPer100;
            p += factor * item.Snapshot.MacrosPer100.ProteinGramsPer100;
            c += factor * item.Snapshot.MacrosPer100.CarbsGramsPer100;
            f += factor * item.Snapshot.MacrosPer100.FatGramsPer100;
        }

        TotalCalories = (int)Math.Round(kcal, MidpointRounding.AwayFromZero);
        TotalProtein = Math.Round(p, 2, MidpointRounding.AwayFromZero);
        TotalCarbohydrates = Math.Round(c, 2, MidpointRounding.AwayFromZero);
        TotalFats = Math.Round(f, 2, MidpointRounding.AwayFromZero);
    }

    private static string NormalizeName(string name)
    {
        var value = name.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                DomainErrorCodes.Meals.InvalidName,
                "Meal name cannot be empty.");
        }
        if (value.Length > DomainConstraints.Meals.MaxMealNameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Meals.InvalidName,
                $"Meal name cannot be longer than {DomainConstraints.Meals.MaxMealNameLength} characters.");
        }
        return value;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }
        var value = description.Trim();
        if (value.Length > DomainConstraints.Meals.MaxMealDescriptionLength)
        {
            throw new DomainException(
                DomainErrorCodes.Meals.InvalidDescription,
                $"Meal description cannot be longer than {DomainConstraints.Meals.MaxMealDescriptionLength} characters.");
        }
        return value;
    }

    private static DateOnly NormalizeLocalDate(DateOnly date)
    {
        if (date == default)
        {
            throw new DomainException(DomainErrorCodes.Meals.InvalidLocalDate, "Local date is invalid.");
        }
        return date;
    }

    private static DateTime EnsureUtc(DateTime dateTime) =>
        dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

    private static DateTimeOffset NormalizeLocal(DateTimeOffset local) => local;
}