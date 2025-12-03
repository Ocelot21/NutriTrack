using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Me.Common;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Mappings;

public static class UserMappings
{
    public static UserResult ToUserResult(this User user)
    {
        return new UserResult(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.IsEmailVerified,
            user.AvatarUrl,
            user.TimeZoneId,
            user.LastLoginAtUtc,
            user.Country,
            user.IsHealthProfileCompleted,
            user.Gender,
            user.ActivityLevel,
            user.Birthdate,
            user.HeightCm,
            user.WeightKg
        );
    }

    public static DailyOverviewResult ToDailyOverviewResult(
        IReadOnlyList<Meal> meals,
        IReadOnlyList<UserExerciseLog> exercises,
        DailyNutritionTargets targets,
        DailyNutritionSnapshot snapshot)
    {
        return new DailyOverviewResult(
            meals.Select(meal => meal.ToMealResult()).ToList(),
            exercises.Select(exercise => exercise.ToUserExerciseLogResult()).ToList(),
            targets,
            snapshot
        );
    }
}