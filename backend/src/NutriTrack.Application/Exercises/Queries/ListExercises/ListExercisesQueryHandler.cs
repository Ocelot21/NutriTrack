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
        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            var list = await _exerciseRepository.ListAsync(
                page: request.Page.Value,
                pageSize: request.PageSize.Value,
                cancellationToken: cancellationToken);

            return new PagedResult<ExerciseResult>(
                list.Items.ToList().Select(exercise => exercise.ToExerciseResult()).ToList(),
                list.TotalCount,
                list.Page,
                list.PageSize);
        }

        var listAll = (await _exerciseRepository.ListAsync(cancellationToken: cancellationToken)).ToList();

        return new PagedResult<ExerciseResult>(
            listAll.Select(exercise => exercise.ToExerciseResult()).ToList(),
            TotalCount: 0,
            Page: 0,
            PageSize: 0);
    }
}