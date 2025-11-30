using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.UserExercises;

public sealed class ExerciseSnapshot : ValueObject
{
    public string ExerciseName { get; private set; } = null!;
    public ExerciseCategory Category { get; private set; }
    public decimal CaloriesPerMinute { get; private set; }

    [Obsolete("Constructor for EF Core only", error: false)]
    private ExerciseSnapshot()
    {
    }

    private ExerciseSnapshot(
        string exerciseName,
        ExerciseCategory category,
        decimal caloriesPerMinute)
    {
        ExerciseName = NormalizeName(exerciseName);
        Category = category;
        CaloriesPerMinute = ValidateCalories(caloriesPerMinute);
    }

    public static ExerciseSnapshot Create(
        string exerciseName,
        ExerciseCategory category,
        decimal caloriesPerMinute)
    {
        return new ExerciseSnapshot(exerciseName, category, caloriesPerMinute);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return ExerciseName;
        yield return Category;
        yield return CaloriesPerMinute;
    }

    private static string NormalizeName(string name)
    {
        var value = name.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidName,
                "Exercise name cannot be empty.");
        }

        if (value.Length > DomainConstraints.UserExerciseLogs.MaxExerciseSnapshotNameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidName,
                $"Exercise name cannot be longer than {DomainConstraints.UserExerciseLogs.MaxExerciseSnapshotNameLength} characters.");
        }

        return value;
    }

    private static decimal ValidateCalories(decimal caloriesPerMinute)
    {
        if (caloriesPerMinute < 0 || caloriesPerMinute >= DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute)
        {
            throw new DomainException(
                DomainErrorCodes.Exercises.InvalidCaloriesPerMinute,
                $"Calories per minute must be between 0 and less than {DomainConstraints.Exercises.MaxDefaultCaloriesPerMinute}.");
        }

        return caloriesPerMinute;
    }
}
