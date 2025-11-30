using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public sealed class ListExercisesQueryHandler : IRequestHandler<ListExercisesQuery, ErrorOr<IReadOnlyList<ExerciseResult>>>
{
    private readonly IExerciseRepository _exerciseRepository;

    public ListExercisesQueryHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<ExerciseResult>>> Handle(ListExercisesQuery request, CancellationToken cancellationToken)
    {
        var list = request.ApprovedOnly
            ? await _exerciseRepository.GetApprovedAsync(cancellationToken)
            : await _exerciseRepository.ListAsync(cancellationToken);

        return list.Select(e => e.ToExerciseResult()).ToList();
    }
}
