using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.DeleteExercise;

public sealed class DeleteExerciseCommandHandler : IRequestHandler<DeleteExerciseCommand, ErrorOr<Unit>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExerciseCommandHandler(IExerciseRepository exerciseRepository, IUnitOfWork unitOfWork)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        _exerciseRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
