namespace NutriTrack.Notifications.Application.Common.Models;

public sealed record PagedResult<TEntity>(
    IReadOnlyList<TEntity> Items,
    int TotalCount,
    int Page,
    int PageSize);