using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.Common.Mappings;

public static class UserExerciseLogMappings
{
    public static UserExerciseLogResult ToUserExerciseLogResult(this UserExerciseLog log)
    {
        return new UserExerciseLogResult(
            log.Id,
            log.UserId,
            log.ExerciseId,
            log.Snapshot.ExerciseName,
            log.Snapshot.Category,
            log.DurationMinutes,
            log.OccurredAtUtc,
            log.OccurredAtLocal,
            log.LocalDate,
            log.TotalCalories,
            log.Notes);
    }
}
