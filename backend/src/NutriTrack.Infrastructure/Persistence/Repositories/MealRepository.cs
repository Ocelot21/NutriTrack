using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class MealRepository : EfRepository<Meal, MealId>, IMealRepository
{
    public MealRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Meal>> GetByUserAndDateRangeAsync(UserId userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        => await _dbContext.Meals
            .Where(m => m.UserId == userId && m.LocalDate >= from && m.LocalDate <= to)
            .ToListAsync(cancellationToken);
}
