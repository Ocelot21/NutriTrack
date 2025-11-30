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
}
