using NutriTrack.Notifications.Application.Common.Interfaces.Persistence;

namespace NutriTrack.Notifications.Infrastructure.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly NotificationsDbContext _dbContext;

        public UnitOfWork(NotificationsDbContext dbContext) => _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
