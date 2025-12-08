using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.UserGoals.Common;

namespace NutriTrack.Application.UserGoals.Queries.ListUserGoals;

public sealed class ListUserGoalsQueryHandler : IRequestHandler<ListUserGoalsQuery, ErrorOr<PagedResult<UserGoalResult>>>
{
    private readonly IUserGoalRepository _repo;

    public ListUserGoalsQueryHandler(IUserGoalRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<PagedResult<UserGoalResult>>> Handle(ListUserGoalsQuery request, CancellationToken cancellationToken)
    {
        var paged = await _repo.GetPagedAsync(request.UserId, 1, 10);
        return new PagedResult<UserGoalResult>(
            paged.Items.OrderByDescending(g => g.StartDate).Select(g => new UserGoalResult(
                g.Id,
                g.UserId,
                g.Type,
                g.Status,
                g.StartDate,
                g.TargetDate,
                g.StartWeightKg,
                g.TargetWeightKg,
                g.CompletedAtUtc,
                g.FailedAtUtc)).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize);
    }
}
