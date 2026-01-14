using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Exercises.Events;

namespace NutriTrack.Domain.Exercises;

public sealed class Exercise : AggregateRoot<ExerciseId>
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private Exercise() : base()
    {
        // Parameterless constructor required by EF Core
    }

    private Exercise(
        ExerciseId id,
        string name,
        string? description,
        ExerciseCategory category,
        decimal defaultCaloriesPerMinute,
        string ? imageUrl,
        bool isDeleted,
        bool isApproved)
        : base(id)
    {
        Name = name;
        Description = description;
        Category = category;
        DefaultCaloriesPerMinute = defaultCaloriesPerMinute;
        IsDeleted = isDeleted;
        IsApproved = isApproved;
        ImageUrl = imageUrl;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public ExerciseCategory Category { get; private set; }
    public decimal DefaultCaloriesPerMinute { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsDeleted { get; private set; }
    public bool IsApproved { get; private set; }

    public void SetImage(string? imageBlobName)
    {
        ImageUrl = string.IsNullOrWhiteSpace(imageBlobName) ? null : imageBlobName;
    }

    // --------- Factory ---------

    public static Exercise Create(
        string name,
        Optional<string> description,
        ExerciseCategory category,
        decimal defaultCaloriesPerMinute,
        string? imageUrl,
        bool isApproved = true)
    {
        name = NormalizeName(name);
        var normalizedDescription = NormalizeDescription(description.IsSet ? description.Value : null);
        ValidateCategory(category);
        ValidateDefaultCaloriesPerMinute(defaultCaloriesPerMinute);

        return new Exercise(
            new ExerciseId(Guid.NewGuid()),
            name,
            normalizedDescription,
            category,
            defaultCaloriesPerMinute,
            imageUrl,
            isDeleted: false,
            isApproved: isApproved);
    }

    // --------- Domain methods ---------

    public void UpdateDefinition(
        Optional<string> name,
        Optional<string> description,
        Optional<ExerciseCategory> category,
        Optional<decimal> defaultCaloriesPerMinute)
    {
        if (name.IsSet)
        {
            Name = NormalizeName(name.Value);
        }

        if (description.IsSet)
        {
            Description = NormalizeDescription(description.Value);
        }

        if (category.IsSet)
        {
            ValidateCategory(category.Value);
            Category = category.Value;
        }

        if (defaultCaloriesPerMinute.IsSet)
        {
            ValidateDefaultCaloriesPerMinute(defaultCaloriesPerMinute.Value);
            DefaultCaloriesPerMinute = defaultCaloriesPerMinute.Value;
        }
    }

    public void Approve()
    {
        if (IsApproved)
            return;

        IsApproved = true;

        if (CreatedBy is not null)
        {
            RaiseDomainEvent(new ExerciseSuggestionApprovedDomainEvent(
                Id,
                CreatedBy.Value));
        }
    }

    public void MarkDeleted()
    {
        IsDeleted = true;
    }

    // --------- Private helpers ---------

    private static string NormalizeName(string value)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidName,
                "Exercise name cannot be empty.");
        }

        if (value.Length > DomainConstraints.Exercises.MaxNameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidName,
                $"Exercise name cannot be longer than {DomainConstraints.Exercises.MaxNameLength} characters.");
        }

        return value;
    }

    private static string? NormalizeDescription(string? value)
    {
        if (value is null)
        {
            return null;
        }

        value = value.Trim();

        if (value.Length == 0)
        {
            return null;
        }

        if (value.Length > DomainConstraints.Exercises.MaxDescriptionLength)
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidDescription,
                $"Description cannot be longer than {DomainConstraints.Exercises.MaxDescriptionLength} characters.");
        }

        return value;
    }

    private static void ValidateCategory(ExerciseCategory category)
    {
        if (!Enum.IsDefined(category))
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidCategory,
                "Invalid exercise category.");
        }
    }

    private static void ValidateDefaultCaloriesPerMinute(decimal value)
    {
        if (value < 0 || value >= DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute)
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidCaloriesPerMinute,
                $"Default calories per minute must be between 0 and less than {DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute}.");
        }

    }
}
