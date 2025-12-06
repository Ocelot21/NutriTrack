using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Queries.ListExercises;

public sealed class ListExercisesQueryHandler : IRequestHandler<ListExercisesQuery, ErrorOr<PagedResult<ExerciseResult>>>
{
    private readonly IExerciseRepository _exerciseRepository;

    public ListExercisesQueryHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<ErrorOr<PagedResult<ExerciseResult>>> Handle(ListExercisesQuery request, CancellationToken cancellationToken)
    {
        var list = await _exerciseRepository.GetPagedAsync(
            request.Filters,
            request.UserId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return list.ToPagedResult();
    }
}