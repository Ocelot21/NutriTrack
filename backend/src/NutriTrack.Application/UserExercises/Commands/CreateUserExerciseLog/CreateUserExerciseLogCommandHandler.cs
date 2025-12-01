using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;

public sealed class CreateUserExerciseLogCommandHandler : IRequestHandler<CreateUserExerciseLogCommand, ErrorOr<UserExerciseLogResult>>
{
    private readonly IUserExerciseLogRepository _repo;
    private readonly IExerciseRepository _exerciseRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserExerciseLogCommandHandler(IUserExerciseLogRepository repo, IExerciseRepository exerciseRepo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _exerciseRepo = exerciseRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserExerciseLogResult>> Handle(CreateUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepo.GetByIdAsync(request.ExerciseId, cancellationToken);
        if (exercise is null)
        {
            return Error.NotFound(code: "Exercises.NotFound", description: "Exercise was not found.");
        }

        var entity = UserExerciseLog.Create(
            request.UserId,
            exercise,
            request.DurationMinutes,
            request.OccurredAtUtc,
            request.OccurredAtLocal,
            request.LocalDate,
            request.Notes);

        await _repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToUserExerciseLogResult();
    }
}
