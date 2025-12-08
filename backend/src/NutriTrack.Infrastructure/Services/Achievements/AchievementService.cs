using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Achievements;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Services.Achievements;

public sealed class AchievementService : IAchievementService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly IUserExerciseLogRepository _exerciseLogRepository;
    private readonly IMealRepository _mealRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITimeZoneService _timeZoneService;

    public AchievementService(
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IAchievementRepository achievementRepository,
        IUserAchievementRepository userAchievementRepository,
        IUserExerciseLogRepository exerciseLogRepository,
        IMealRepository mealRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ITimeZoneService timeZoneService)
    {
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _achievementRepository = achievementRepository;
        _userAchievementRepository = userAchievementRepository;
        _exerciseLogRepository = exerciseLogRepository;
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _timeZoneService = timeZoneService;
    }

    public async Task CheckGoalCompletedAsync(
        UserId userId,
        CancellationToken ct = default)
    {
        if (await _userRepository.GetByIdAsync(userId, ct) is not User user)
        {
            return;
        }

        DateOnly localDate = GetLocalDateForUser(user);

        var completedCount = await _userGoalRepository.CountCompletedAsync(userId, ct);

        var thresholds = new[] { 1, 5, 10, 20 };

        foreach (var t in thresholds)
        {
            if (completedCount < t)
            {
                continue;
            }

            var key = $"GOALS_COMPLETED_{t}";

            var alreadyHas = await _userAchievementRepository.ExistsAsync(userId, key, ct);
            if (alreadyHas)
            {
                continue;
            }

            var achievement = await _achievementRepository.GetByKeyAsync(key, ct);
            if (achievement is null)
            {
                continue;
            }

            var userAchievement = UserAchievement.Create(
                userId,
                achievement.Id,
                localDate);

            await _userAchievementRepository.AddAsync(userAchievement);
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task CheckExerciseLoggedAsync(
        UserId userId,
        CancellationToken ct = default)
    {
        if (await _userRepository.GetByIdAsync(userId, ct) is not User user)
        {
            return;
        }

        DateOnly localDate = GetLocalDateForUser(user);

        var totalLogs = await _exerciseLogRepository.CountLogsAsync(userId, ct);
        if (totalLogs >= 1)
        {
            await TryUnlockAsync(userId, "EXERCISE_FIRST_LOG", localDate, ct);
        }

        var streakDays = await _exerciseLogRepository.GetCurrentStreakDaysAsync(userId, localDate, ct);

        var streakThresholds = new[] { 3, 7, 30 };

        foreach (var t in streakThresholds)
        {
            if (streakDays >= t)
            {
                await TryUnlockAsync(userId, $"EXERCISE_STREAK_{t}_DAYS", localDate, ct);
            }
        }

        var totalCalories = await _exerciseLogRepository.GetTotalCaloriesAsync(userId, ct);

        var calorieThresholds = new[] { 1000, 10000, 50000 };

        foreach (var t in calorieThresholds)
        {
            if (totalCalories >= t)
            {
                await TryUnlockAsync(userId, $"CALORIES_BURNED_{t}", localDate, ct);
            }
        }
    }

    

    public async Task CheckMealItemLoggedAsync(
        UserId userId,
        CancellationToken ct = default)
    {
        if (await _userRepository.GetByIdAsync(userId, ct) is not User user)
        {
            return;
        }

        DateOnly localDate = GetLocalDateForUser(user);


        var totalItems = await _mealRepository.CountTotalItemsForUserAsync(userId, ct);
        if (totalItems >= 1)
        {
            await TryUnlockAsync(userId, "FIRST_MEAL_ITEM", localDate, ct);
        }

        var streakDays = await _mealRepository.GetCurrentStreakDaysAsync(userId, localDate, ct);
        var streakThresholds = new[] { 3, 7, 30 };

        foreach (var t in streakThresholds)
        {
            if (streakDays >= t)
            {
                await TryUnlockAsync(userId, $"MEAL_ITEM_STREAK_{t}_DAYS", localDate, ct);
            }
        }
    }

    private async Task TryUnlockAsync(
        UserId userId,
        string achievementKey,
        DateOnly localDate,
        CancellationToken ct)
    {
        var alreadyHas = await _userAchievementRepository.ExistsAsync(userId, achievementKey, ct);
        if (alreadyHas)
        {
            return;
        }

        var achievement = await _achievementRepository.GetByKeyAsync(achievementKey, ct);
        if (achievement is null)
        {
            return;
        }

        var userAchievement = UserAchievement.Create(
            userId,
            achievement.Id,
            localDate);

        await _userAchievementRepository.AddAsync(userAchievement, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private DateOnly GetLocalDateForUser(User user)
    {
        if (!_timeZoneService.TryNormalize(user.TimeZoneId, out var tzId))
        {
            tzId = "UTC";
        }

        var localDate = _timeZoneService.LocalDate(_dateTimeProvider.UtcNow, tzId);
        return localDate;
    }
}