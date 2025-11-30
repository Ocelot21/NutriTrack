using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class UserExerciseLogRepository : EfRepository<UserExerciseLog, UserExerciseLogId>, IUserExerciseLogRepository
{
    public UserExerciseLogRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<UserExerciseLog>> GetByUserAndDateRangeAsync(UserId userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        => await _dbContext.UserExerciseLogs
            .Where(l => l.UserId == userId && l.LocalDate >= from && l.LocalDate <= to)
            .ToListAsync(cancellationToken);
}
