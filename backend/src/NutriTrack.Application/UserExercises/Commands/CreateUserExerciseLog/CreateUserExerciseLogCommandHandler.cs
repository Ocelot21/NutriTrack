using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.UserExercises.Common;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.UserExercises.Commands.CreateUserExerciseLog;

public sealed class CreateUserExerciseLogCommandHandler : IRequestHandler<CreateUserExerciseLogCommand, ErrorOr<UserExerciseLogResult>>
{
    private readonly IUserExerciseLogRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserExerciseLogCommandHandler(IUserExerciseLogRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UserExerciseLogResult>> Handle(CreateUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        var entity = UserExerciseLog.Create(
            request.UserId,
            request.Exercise,
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
