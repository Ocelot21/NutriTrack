using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Common
{
    public sealed record ExerciseListFilters(
    string? SearchTerm,
    ExerciseCategory? Category,
    decimal? MinCaloriesPerMinute,
    decimal? MaxCaloriesPerMinute
);

}
