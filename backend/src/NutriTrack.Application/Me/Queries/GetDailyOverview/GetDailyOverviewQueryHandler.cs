using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Services;
using NutriTrack.Application.Me.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Queries.GetDailyOverview;

public sealed class GetDailyOverviewQueryHandler : IRequestHandler<GetDailyOverviewQuery, ErrorOr<DailyOverviewResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMealRepository _mealRepository;
    private readonly IUserExerciseLogRepository _userExerciseLogRepository;

    public GetDailyOverviewQueryHandler(
        IUserRepository userRepository,
        IMealRepository mealRepository,
        IUserExerciseLogRepository userExerciseLogRepository)
    {
        _userRepository = userRepository;
        _mealRepository = mealRepository;
        _userExerciseLogRepository = userExerciseLogRepository;
    }

    public async Task<ErrorOr<DailyOverviewResult>> Handle(GetDailyOverviewQuery request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId) is not User user)
        {
            return Errors.Users.NotFound;
        }

        var meals = await _mealRepository.GetByUserAndDateRangeAsync(
            user.Id,
            request.Date,
            request.Date,
            cancellationToken);

        var userExerciseLogs = await _userExerciseLogRepository.GetByUserAndDateRangeAsync(
            user.Id,
            request.Date,
            request.Date,
            cancellationToken);

        if (!user.IsHealthProfileCompleted)
            return Errors.Users.HealthProfileNotCompleted;

        if (!user.Birthdate.HasValue)
            return Errors.Users.HealthProfileNotCompleted;

        if (!user.WeightKg.HasValue)
            return Errors.Users.HealthProfileNotCompleted;

        if (!user.HeightCm.HasValue)
            return Errors.Users.HealthProfileNotCompleted;

        int ageYears = request.Date.Year - user.Birthdate.Value.Year;

        if (user.Birthdate.Value > new DateOnly(
            request.Date.Year,
            user.Birthdate.Value.Month,
            user.Birthdate.Value.Day))
            ageYears--;

        var context = new DailyNutritionContext(
            user.Gender,
            ageYears,
            user.HeightCm!.Value,
            user.WeightKg!.Value,
            user.ActivityLevel, 
            user.NutritionGoal);

        var targets = DailyNutritionCalculator.CalculateGoals(context);

        var consumedCalories = meals.Sum(meal => meal.TotalCalories);
        var burnedCalories = (int)userExerciseLogs.Sum(exercise => exercise.TotalCalories);
        var netCalories = consumedCalories - burnedCalories;
        var remainingCalories = targets.Calories - netCalories;

        var consumedProteinGrams = meals.Sum(meal => meal.TotalProtein);
        var consumedFatGrams = meals.Sum(meal => meal.TotalFats);
        var consumedCarbohydrateGrams = meals.Sum(meal => meal.TotalCarbohydrates);

        var snapshot = new DailyNutritionSnapshot(
            consumedCalories,
            burnedCalories,
            netCalories,
            remainingCalories,
            consumedProteinGrams,
            consumedFatGrams,
            consumedCarbohydrateGrams);

        return UserMappings.ToDailyOverviewResult(meals, userExerciseLogs, targets, snapshot);
    }
}