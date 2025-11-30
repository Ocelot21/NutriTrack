using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IRepository<TEntity, TId>
where TEntity : AggregateRoot<TId>
where TId : struct
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<TEntity>> ListAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Remove(TEntity entity);
}