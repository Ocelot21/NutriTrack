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
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly IActivityLevelHistoryRepository _activityLevelHistoryRepository;

    public GetDailyOverviewQueryHandler(
        IUserRepository userRepository,
        IMealRepository mealRepository,
        IUserExerciseLogRepository userExerciseLogRepository,
        IUserGoalRepository userGoalRepository,
        IWeightHistoryRepository weightHistoryRepository,
        IActivityLevelHistoryRepository activityLevelHistoryRepository)
    {
        _userRepository = userRepository;
        _mealRepository = mealRepository;
        _userExerciseLogRepository = userExerciseLogRepository;
        _userGoalRepository = userGoalRepository;
        _weightHistoryRepository = weightHistoryRepository;
        _activityLevelHistoryRepository = activityLevelHistoryRepository;
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

        var relevantGoal = await _userGoalRepository.GetClosestOnOrBeforeAsync(user.Id, request.Date, cancellationToken);
        var nutritionGoal = relevantGoal?.Type ?? user.NutritionGoal;

        var relevantWeightEntry = await _weightHistoryRepository.GetClosestOnOrBeforeAsync(user.Id, request.Date, cancellationToken);
        var weightKg = relevantWeightEntry?.WeightKg ?? user.WeightKg!.Value;

        var relevantActivityLevelEntry = await _activityLevelHistoryRepository.GetClosestOnOrBeforeAsync(user.Id, request.Date, cancellationToken);
        var activityLevel = relevantActivityLevelEntry?.ActivityLevel ?? user.ActivityLevel;

        var context = new DailyNutritionContext(
            user.Gender,
            ageYears,
            user.HeightCm!.Value,
            weightKg,
            activityLevel,
            nutritionGoal);

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