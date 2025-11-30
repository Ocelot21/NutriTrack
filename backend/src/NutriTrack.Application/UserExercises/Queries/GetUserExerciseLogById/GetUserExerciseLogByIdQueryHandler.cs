using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.UserExercises.Common;

namespace NutriTrack.Application.UserExercises.Queries.GetUserExerciseLogById;

public sealed class GetUserExerciseLogByIdQueryHandler : IRequestHandler<GetUserExerciseLogByIdQuery, ErrorOr<UserExerciseLogResult>>
{
    private readonly IUserExerciseLogRepository _repo;

    public GetUserExerciseLogByIdQueryHandler(IUserExerciseLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<UserExerciseLogResult>> Handle(GetUserExerciseLogByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }
        return entity.ToUserExerciseLogResult();
    }
}
