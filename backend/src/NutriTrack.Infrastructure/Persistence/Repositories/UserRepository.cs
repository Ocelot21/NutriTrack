using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories
{
    internal sealed class UserRepository : EfRepository<User, UserId>, IUserRepository
    {

        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> IsUsernameTakenAsync(Username username, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .AnyAsync(u => u.Username == username, cancellationToken);
        }
    }
}