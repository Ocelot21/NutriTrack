using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.UserExercises;

namespace NutriTrack.Application.UserExercises.Commands.DeleteUserExerciseLog;

public sealed class DeleteUserExerciseLogCommandHandler : IRequestHandler<DeleteUserExerciseLogCommand, ErrorOr<Unit>>
{
    private readonly IUserExerciseLogRepository _userExerciseLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserExerciseLogCommandHandler(
        IUserExerciseLogRepository userExerciseLogRepository, 
        IUnitOfWork unitOfWork)
    {
        _userExerciseLogRepository = userExerciseLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        if (await _userExerciseLogRepository
            .GetByIdAsync(request.Id, cancellationToken) is not UserExerciseLog exerciseLog)
        {
            return Errors.Exercises.NotFound;
        }

        _userExerciseLogRepository.Remove(exerciseLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
