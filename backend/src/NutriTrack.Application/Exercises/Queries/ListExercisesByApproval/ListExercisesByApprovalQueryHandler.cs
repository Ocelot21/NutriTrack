using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Application.Common.Mappings;

namespace NutriTrack.Application.Exercises.Queries.ListExercisesByApproval;

public sealed class ListExercisesByApprovalQueryHandler : IRequestHandler<ListExercisesByApprovalQuery, ErrorOr<PagedResult<ExerciseResult>>>
{
    private readonly IExerciseRepository _repo;

    public ListExercisesByApprovalQueryHandler(IExerciseRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<PagedResult<ExerciseResult>>> Handle(ListExercisesByApprovalQuery request, CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedByApprovalAsync(request.IsApproved, request.Page, request.PageSize, cancellationToken);
        return new PagedResult<ExerciseResult>(
            paged.Items.Select(e => e.ToExerciseResult()).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize);
    }
}
