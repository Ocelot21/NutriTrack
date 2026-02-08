using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IRepository<TEntity, TId>
where TEntity : AggregateRoot<TId>
where TId : notnull
{
    Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<PagedResult<TEntity>> ListAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<bool> ExistsAsync(
        TId id,
        CancellationToken cancellationToken = default);
}