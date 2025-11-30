using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.UserExercises.Common;

namespace NutriTrack.Application.UserExercises.Queries.ListUserExerciseLogs;

public sealed class ListUserExerciseLogsQueryHandler : IRequestHandler<ListUserExerciseLogsQuery, ErrorOr<IReadOnlyList<UserExerciseLogResult>>>
{
    private readonly IUserExerciseLogRepository _repo;

    public ListUserExerciseLogsQueryHandler(IUserExerciseLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<IReadOnlyList<UserExerciseLogResult>>> Handle(ListUserExerciseLogsQuery request, CancellationToken cancellationToken)
    {
        if (request.From.HasValue && request.To.HasValue)
        {
            var list = await _repo.GetByUserAndDateRangeAsync(request.UserId, request.From.Value, request.To.Value, cancellationToken);
            return list.Select(l => l.ToUserExerciseLogResult()).ToList();
        }

        var all = await _repo.ListAsync(cancellationToken);
        return all.Where(l => l.UserId == request.UserId).Select(l => l.ToUserExerciseLogResult()).ToList();
    }
}
