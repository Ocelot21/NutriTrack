using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Exercises;
using NutriTrack.Domain.Meals;
using NutriTrack.Domain.UserExercises;
using NutriTrack.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository : EfRepository<Role, RoleId>, IRoleRepository
{
    public RoleRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbContext.Roles.AnyAsync(r => r.Name == name, cancellationToken);

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
