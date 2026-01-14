using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.WeightHistory;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.WeightHistoryEntries.Commands.CreateWeightHistoryEntry;

public sealed class CreateWeightHistoryEntryCommandHandler : IRequestHandler<CreateWeightHistoryEntryCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IAchievementService _achievementService;

    public CreateWeightHistoryEntryCommandHandler(
        IUserRepository userRepository,
        IWeightHistoryRepository weightHistoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IUserGoalRepository userGoalRepository,
        ITimeZoneService timeZoneService,
        IAchievementService achievementService)
    {
        _userRepository = userRepository;
        _weightHistoryRepository = weightHistoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _userGoalRepository = userGoalRepository;
        _timeZoneService = timeZoneService;
        _achievementService = achievementService;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateWeightHistoryEntryCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var entry = WeightHistoryEntry.Create(request.UserId, request.Date, request.WeightKg, utcNow);

        user.UpdateHealthProfile(
            gender: Optional<Domain.Users.Gender>.None(),
            birthdate: Optional<DateOnly?>.None(),
            heightCm: Optional<decimal?>.None(),
            weightKg: request.WeightKg,
            activityLevel: Optional<Domain.Users.ActivityLevel>.None(),
            nutritionGoal: Optional<Domain.Users.NutritionGoal>.None());

        await _weightHistoryRepository.AddAsync(entry, cancellationToken);

        var currentGoal = await _userGoalRepository.GetCurrentForUser(request.UserId);
        if (currentGoal is not null)
        {
            var currentDate = _timeZoneService.LocalDate(utcNow, user.TimeZoneId);
            var changed = currentGoal.EvaluateProgress(request.WeightKg, currentDate, utcNow);
            if (changed)
            {
                await _achievementService.CheckGoalCompletedAsync(user.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}