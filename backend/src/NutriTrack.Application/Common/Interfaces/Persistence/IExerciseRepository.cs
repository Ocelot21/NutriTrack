using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IExerciseRepository : IRepository<Exercise, ExerciseId>
{
    Task<IReadOnlyList<Exercise>> GetApprovedAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<Exercise>> GetPagedAsync(
        ExerciseListFilters filters,
        UserId? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Exercise>> GetPagedByApprovalAsync(
        bool isApproved,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
