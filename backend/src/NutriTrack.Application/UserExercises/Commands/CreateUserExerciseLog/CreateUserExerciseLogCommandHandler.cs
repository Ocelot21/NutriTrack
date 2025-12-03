using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;

public sealed class CreateUserExerciseLogCommandHandler : IRequestHandler<CreateUserExerciseLogCommand, ErrorOr<UserExerciseLogResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserExerciseLogRepository _userExerciseRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ITimeZoneService _timeZoneService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserExerciseLogCommandHandler(
        IUserRepository userRepository,
        IUserExerciseLogRepository userExerciseLogsRepository,
        IExerciseRepository exerciseRepository,
        ITimeZoneService timeZoneService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userExerciseRepository = userExerciseLogsRepository;
        _exerciseRepository = exerciseRepository;
        _timeZoneService = timeZoneService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserExerciseLogResult>> Handle(CreateUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) is not User user)
        {
            return Errors.Users.NotFound;
        }

        if (await _exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken) is not Exercise exercise)
        {
            return Errors.Exercises.NotFound;
        }

        if (!_timeZoneService.TryNormalize(user.TimeZoneId, out string normalizedTimeZoneId))
        {
            return Errors.Users.InvalidTimeZone;
        }

        var occuredAtUtc = _timeZoneService.ToUtc(request.OccurredAtLocal, normalizedTimeZoneId);

        var localDate = DateOnly.FromDateTime(request.OccurredAtLocal.LocalDateTime);

        var entity = UserExerciseLog.Create(
            request.UserId,
            exercise,
            request.DurationMinutes,
            occuredAtUtc,
            request.OccurredAtLocal,
            localDate,
            request.Notes);

        await _userExerciseRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToUserExerciseLogResult();
    }
}