using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Queries.GetExerciseById;

public sealed class GetExerciseByIdQueryHandler : IRequestHandler<GetExerciseByIdQuery, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;

    public GetExerciseByIdQueryHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        return entity.ToExerciseResult();
    }
}
