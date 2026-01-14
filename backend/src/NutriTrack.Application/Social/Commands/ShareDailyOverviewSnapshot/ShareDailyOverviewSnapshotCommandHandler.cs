using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Services;
using NutriTrack.Domain.Social;
using NutriTrack.Domain.Social.Snapshots;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Social.Commands.ShareDailyOverviewSnapshot;

public sealed class ShareDailyOverviewSnapshotCommandHandler : IRequestHandler<ShareDailyOverviewSnapshotCommand, ErrorOr<Unit>>
{
    private readonly ISocialPostRepository _socialPostRepository;
    private readonly IDailyOverviewSnapshotRepository _dailyOverviewSnapshotRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMealRepository _mealRepository;
    private readonly IUserExerciseLogRepository _userExerciseLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShareDailyOverviewSnapshotCommandHandler(
        ISocialPostRepository socialPostRepository,
        IDailyOverviewSnapshotRepository dailyOverviewSnapshotRepository,
        IUserRepository userRepository,
        IMealRepository mealRepository,
        IUserExerciseLogRepository userExerciseLogRepository,
        IUnitOfWork unitOfWork)
    {
        _socialPostRepository = socialPostRepository;
        _dailyOverviewSnapshotRepository = dailyOverviewSnapshotRepository;
        _userRepository = userRepository;
        _mealRepository = mealRepository;
        _userExerciseLogRepository = userExerciseLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(ShareDailyOverviewSnapshotCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        if (await _userRepository.GetByIdAsync(userId, cancellationToken) is not User user)
        {
            return Errors.Users.NotFound;
        }

        var meals = await _mealRepository.GetByUserAndDateRangeAsync(
            user.Id,
            request.LocalDate,
            request.LocalDate,
            cancellationToken);

        var userExerciseLogs = await _userExerciseLogRepository.GetByUserAndDateRangeAsync(
            user.Id,
            request.LocalDate,
            request.LocalDate,
            cancellationToken);

        if (!user.IsHealthProfileCompleted ||
            !user.Birthdate.HasValue ||
            !user.WeightKg.HasValue ||
            !user.HeightCm.HasValue)
        {
            return Errors.Users.HealthProfileNotCompleted;
        }

        int ageYears = request.LocalDate.Year - user.Birthdate.Value.Year;

        if (user.Birthdate.Value > new DateOnly(
                request.LocalDate.Year,
                user.Birthdate.Value.Month,
                user.Birthdate.Value.Day))
        {
            ageYears--;
        }

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

        var snapshot = new DailyOverviewSnapshot(
            new DailyOverviewSnapshotId(Guid.NewGuid()),
            request.LocalDate,
            targetCalories: targets.Calories,
            targetProteinGrams: targets.ProteinGrams,
            targetFatGrams: targets.FatGrams,
            targetCarbohydrateGrams: targets.CarbohydrateGrams,
            consumedCalories: consumedCalories,
            burnedCalories: burnedCalories,
            netCalories: netCalories,
            remainingCalories: remainingCalories,
            consumedProteinGrams: consumedProteinGrams,
            consumedFatGrams: consumedFatGrams,
            consumedCarbohydrateGrams: consumedCarbohydrateGrams,
            mealCount: meals.Count,
            exerciseCount: userExerciseLogs.Count);

        await _dailyOverviewSnapshotRepository.AddAsync(snapshot, cancellationToken);

        var visibility = request.Visibility.HasValue ? (PostVisibility)request.Visibility.Value : PostVisibility.Public;
        var post = SocialPost.ShareDailyOverviewSnapshot(userId, snapshot.Id, request.Caption, visibility);

        await _socialPostRepository.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
