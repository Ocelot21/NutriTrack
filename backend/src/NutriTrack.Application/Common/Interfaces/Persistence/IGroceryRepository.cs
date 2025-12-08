using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Groceries;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IGroceryRepository : IRepository<Grocery, GroceryId>
{
    Task<IReadOnlyList<Grocery>> SearchByNameAsync(string nameFragment, CancellationToken cancellationToken = default);
    Task<Grocery?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<PagedResult<Grocery>> GetPagedAsync(
        GroceryListFilters filters,
        UserId? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Grocery>> GetPagedByApprovalAsync(
        bool isApproved,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
