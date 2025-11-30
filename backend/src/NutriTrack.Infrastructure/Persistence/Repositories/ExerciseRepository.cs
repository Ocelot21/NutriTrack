using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Exercises;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class ExerciseRepository : EfRepository<Exercise, ExerciseId>, IExerciseRepository
{
    public ExerciseRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Exercise>> GetApprovedAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Exercises
            .Where(e => e.IsApproved && !e.IsDeleted)
            .ToListAsync(cancellationToken);
}
