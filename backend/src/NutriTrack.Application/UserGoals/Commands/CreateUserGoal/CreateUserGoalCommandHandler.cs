using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.UserGoals.Common;
using NutriTrack.Domain.UserGoals;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.UserGoals.Commands.CreateUserGoal;

public sealed class CreateUserGoalCommandHandler : IRequestHandler<CreateUserGoalCommand, ErrorOr<UserGoalResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITimeZoneService _timeZoneService;

    public CreateUserGoalCommandHandler(
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ITimeZoneService timeZoneService)
    {
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _timeZoneService = timeZoneService;
    }

    public async Task<ErrorOr<UserGoalResult>> Handle(
        CreateUserGoalCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }
        if (!user.WeightKg.HasValue)
        {
            return Errors.Users.HealthProfileNotCompleted;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var localDate = _timeZoneService.LocalDate(utcNow, user.TimeZoneId);

        var startWeightKg = user.WeightKg.Value;

        user.UpdateHealthProfile(
            gender: Optional<Domain.Users.Gender>.None(),
            birthdate: Optional<DateOnly?>.None(),
            heightCm: Optional<decimal?>.None(),
            weightKg: Optional<decimal?>.None(),
            activityLevel: Optional<Domain.Users.ActivityLevel>.None(),
            nutritionGoal: request.Type);

        var goal = UserGoal.Create(
            user.Id,
            request.Type,
            localDate,
            request.TargetDate,
            startWeightKg,
            request.TargetWeightKg);

        await _userGoalRepository.AddAsync(goal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new UserGoalResult(
            goal.Id,
            goal.UserId,
            goal.Type,
            goal.Status,
            goal.StartDate,
            goal.TargetDate,
            goal.StartWeightKg,
            goal.TargetWeightKg,
            goal.CompletedAtUtc,
            goal.FailedAtUtc);
    }
}