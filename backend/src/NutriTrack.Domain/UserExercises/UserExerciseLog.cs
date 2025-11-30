using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Users;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.UserExercises;

public sealed class UserExerciseLog : AggregateRoot<UserExerciseLogId>
{
    private UserExerciseLog() : base() { }

    private UserExerciseLog(
        UserExerciseLogId id,
        UserId userId,
        Exercise exercise,
        decimal durationMinutes,
        DateTime occurredAtUtc,
        DateTimeOffset occurredAtLocal,
        DateOnly localDate,
        string? notes)
        : base(id)
    {
        UserId = userId;
        ExerciseId = exercise.Id;

        DurationMinutes = durationMinutes;
        OccurredAtUtc = occurredAtUtc;
        OccurredAtLocal = occurredAtLocal;
        LocalDate = localDate;
        Notes = notes;

        Snapshot = ExerciseSnapshot.Create(
            exercise.Name,
            exercise.Category,
            exercise.DefaultCaloriesPerMinute);

        RecomputeTotalCalories();
    }

    public UserId UserId { get; private set; }
    public User? User { get; private set; } = null!;

    public ExerciseId ExerciseId { get; private set; }
    public Exercise? Exercise { get; private set; } = null!;

    public ExerciseSnapshot Snapshot { get; private set; } = null!;

    public decimal DurationMinutes { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public DateTimeOffset OccurredAtLocal { get; private set; }
    public DateOnly LocalDate { get; private set; }

    public decimal TotalCalories { get; private set; }
    public string? Notes { get; private set; }

    public static UserExerciseLog Create(
        UserId userId,
        Exercise exercise,
        decimal durationMinutes,
        DateTime occurredAtUtc,
        DateTimeOffset occurredAtLocal,
        DateOnly localDate,
        string? notes)
    {
        if (userId.Value == Guid.Empty)
        {
            throw new DomainException(DomainErrorCodes.UserExerciseLogs.InvalidUser, "User identifier is required.");
        }

        if (exercise is null)
        {
            throw new DomainException(DomainErrorCodes.UserExerciseLogs.InvalidExercise, "Exercise is required.");
        }

        var validDuration = ValidateDuration(durationMinutes);
        var utc = EnsureUtc(occurredAtUtc);
        var local = NormalizeLocal(occurredAtLocal);
        var localDateNorm = NormalizeLocalDate(localDate);
        var normalizedNotes = NormalizeNotes(notes);

        return new UserExerciseLog(
            new UserExerciseLogId(Guid.NewGuid()),
            userId,
            exercise,
            validDuration,
            utc,
            local,
            localDateNorm,
            normalizedNotes);
    }

    public void Update(
        Optional<decimal> durationMinutes,
        Optional<DateTime?> occurredAtUtc,
        Optional<DateTimeOffset?> occurredAtLocal,
        Optional<DateOnly?> localDate,
        Optional<string?> notes)
    {
        if (durationMinutes.IsSet)
        {
            DurationMinutes = ValidateDuration(durationMinutes.Value);
        }

        if (occurredAtUtc.IsSet && occurredAtUtc.Value is not null)
        {
            OccurredAtUtc = EnsureUtc(occurredAtUtc.Value.Value);
        }

        if (occurredAtLocal.IsSet && occurredAtLocal.Value is not null)
        {
            OccurredAtLocal = NormalizeLocal(occurredAtLocal.Value.Value);
        }

        if (localDate.IsSet && localDate.Value is not null)
        {
            LocalDate = NormalizeLocalDate(localDate.Value.Value);
        }

        if (notes.IsSet)
        {
            Notes = NormalizeNotes(notes.Value);
        }

        RecomputeTotalCalories();
    }

    private static decimal ValidateDuration(decimal durationMinutes)
    {
        if (durationMinutes <= 0 || durationMinutes > 1440)
        {
            throw new DomainException(DomainErrorCodes.UserExerciseLogs.InvalidDuration, "Exercise duration must be between 0 and 1440 minutes.");
        }
        return durationMinutes;
    }

    private static DateOnly NormalizeLocalDate(DateOnly date)
    {
        if (date == default)
        {
            throw new DomainException(DomainErrorCodes.UserExerciseLogs.InvalidLocalDate, "Local date is required.");
        }
        return date;
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return null;
        }
        var value = notes.Trim();
        if (value.Length > DomainConstraints.UserExerciseLogs.MaxNotesLength)
        {
            throw new DomainException(DomainErrorCodes.UserExerciseLogs.InvalidNotes, $"Notes cannot be longer than {DomainConstraints.UserExerciseLogs.MaxNotesLength} characters.");
        }
        return value;
    }

    private static DateTime EnsureUtc(DateTime dateTime) =>
        dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

    private static DateTimeOffset NormalizeLocal(DateTimeOffset local) => local;

    private void RecomputeTotalCalories()
    {
        TotalCalories = Math.Round(DurationMinutes * Snapshot.CaloriesPerMinute, 2, MidpointRounding.AwayFromZero);
    }
}
