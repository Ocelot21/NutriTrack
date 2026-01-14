using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
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

        public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == Username.Create(username), cancellationToken);
        }

        public Task<bool> IsUsernameTakenAsync(Username username, CancellationToken cancellationToken = default)
        {
            return _dbContext.Users
                .AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _dbContext.Users
                .OrderBy(u => u.Username)
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>(items, totalCount, page, pageSize);
        }
    }
}