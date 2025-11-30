using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IExerciseRepository : IRepository<Exercise, ExerciseId>
{
    Task<IReadOnlyList<Exercise>> GetApprovedAsync(CancellationToken cancellationToken = default);
}
