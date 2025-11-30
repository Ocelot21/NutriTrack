using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IUserExerciseLogRepository : IRepository<UserExerciseLog, UserExerciseLogId>
{
    Task<IReadOnlyList<UserExerciseLog>> GetByUserAndDateRangeAsync(UserId userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
}
