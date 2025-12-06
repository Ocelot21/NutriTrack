using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Common.Mappings;

public static class ExerciseMappings
{
    public static ExerciseResult ToExerciseResult(this Exercise exercise)
    {
        return new ExerciseResult(
            exercise.Id,
            exercise.Name,
            exercise.Category,
            exercise.DefaultCaloriesPerMinute,
            exercise.Description,
            exercise.ImageUrl,
            exercise.IsApproved,
            exercise.IsDeleted);
    }

    public static PagedResult<ExerciseResult> ToPagedResult(this PagedResult<Exercise> pagedExercises)
    {
        var exerciseResults = pagedExercises.Items
            .Select(exercise => exercise.ToExerciseResult())
            .ToList();
        return new PagedResult<ExerciseResult>(
            exerciseResults,
            pagedExercises.TotalCount,
            pagedExercises.Page,
            pagedExercises.PageSize);
    }
}