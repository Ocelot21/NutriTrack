using ErrorOr;
using MediatR;

namespace NutriTrack.Application.Me.Queries.GetDailyOverview;

public sealed class GetDailyOverviewQueryHandler : IRequestHandler<GetDailyOverviewQuery, ErrorOr<Unit>>
{
    public Task<ErrorOr<Unit>> Handle(GetDailyOverviewQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<ErrorOr<Unit>>(Unit.Value);
    }
}
