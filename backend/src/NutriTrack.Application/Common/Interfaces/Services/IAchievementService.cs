using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Services;

public interface IAchievementService
{
    Task CheckGoalCompletedAsync(UserId userId, CancellationToken ct = default);
    Task CheckExerciseLoggedAsync(UserId userId, CancellationToken ct = default);
    Task CheckMealItemLoggedAsync(UserId userId, CancellationToken ct = default);
}

