using NutriTrack.Domain.Authorization;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
