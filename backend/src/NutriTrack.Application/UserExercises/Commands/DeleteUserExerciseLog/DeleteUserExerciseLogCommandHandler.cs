using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;

public sealed class DeleteUserExerciseLogCommandHandler : IRequestHandler<DeleteUserExerciseLogCommand, ErrorOr<Unit>>
{
    private readonly IUserExerciseLogRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserExerciseLogCommandHandler(IUserExerciseLogRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        _repo.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
