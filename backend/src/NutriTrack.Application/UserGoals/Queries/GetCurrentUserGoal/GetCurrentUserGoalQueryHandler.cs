using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.UserGoals.Common;

namespace NutriTrack.Application.UserGoals.Queries.GetCurrentUserGoal;

public sealed class GetCurrentUserGoalQueryHandler : IRequestHandler<GetCurrentUserGoalQuery, ErrorOr<UserGoalResult>>
{
    private readonly IUserGoalRepository _userGoalRepository;

    public GetCurrentUserGoalQueryHandler(IUserGoalRepository userGoalRepository)
    {
        _userGoalRepository = userGoalRepository;
    }

    public async Task<ErrorOr<UserGoalResult>> Handle(GetCurrentUserGoalQuery request, CancellationToken cancellationToken)
    {
        var goal = await _userGoalRepository.GetCurrentForUser(request.UserId);
        if (goal is null)
        {
            return Errors.UserGoals.NotFound;
        }

        return new UserGoalResult(
            goal.Id,
            goal.UserId,
            goal.Type,
            goal.Status,
            goal.StartDate,
            goal.TargetDate,
            goal.StartWeightKg,
            goal.TargetWeightKg,
            goal.CompletedAtUtc,
            goal.FailedAtUtc);
    }
}
