using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;

namespace NutriTrack.Application.UserGoals.Commands.EvaluateUserProgress;

public sealed class EvaluateUserProgressCommandHandler : IRequestHandler<EvaluateUserProgressCommand, ErrorOr<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IUnitOfWork _unitOfWork;

    public EvaluateUserProgressCommandHandler(
        IUserRepository userRepository,
        IUserGoalRepository userGoalRepository,
        IDateTimeProvider dateTimeProvider,
        ITimeZoneService timeZoneService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userGoalRepository = userGoalRepository;
        _dateTimeProvider = dateTimeProvider;
        _timeZoneService = timeZoneService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(EvaluateUserProgressCommand request, CancellationToken cancellationToken)
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

        var goal = await _userGoalRepository.GetByIdAsync(request.GoalId, cancellationToken);
        if (goal is null)
        {
            return Errors.UserGoals.NotFound;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var currentDate = _timeZoneService.LocalDate(utcNow, user.TimeZoneId);
        var currentWeightKg = user.WeightKg.Value;

        var changed = goal.EvaluateProgress(currentWeightKg, currentDate, utcNow);
        if (changed)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return changed;
    }
}
