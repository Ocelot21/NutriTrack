using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Commands.ApproveExercise;

public sealed class ApproveExerciseCommandHandler
    : IRequestHandler<ApproveExerciseCommand, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveExerciseCommandHandler(IExerciseRepository exerciseRepository, IUnitOfWork unitOfWork)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(ApproveExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (exercise is null)
        {
            return Errors.Exercises.NotFound;
        }

        exercise.Approve();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return exercise.ToExerciseResult();
    }
}
