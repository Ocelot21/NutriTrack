using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameTakenAsync(Username username, CancellationToken cancellationToken = default);
}