using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.Exercises.Commands.CreateExercise;

public sealed class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExerciseCommandHandler(IExerciseRepository exerciseRepository, IUnitOfWork unitOfWork)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        var descriptionOpt = request.Description is null
            ? Optional<string>.None()
            : request.Description;

        var entity = Exercise.Create(
            request.Name,
            descriptionOpt,
            request.Category,
            request.DefaultCaloriesPerMinute,
            request.ImageUrl,
            isApproved: true);

        await _exerciseRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToExerciseResult();
    }
}
