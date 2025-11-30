using NutriTrack.Domain.Groceries;

namespace NutriTrack.Application.Common.Interfaces.Persistence;

public interface IGroceryRepository : IRepository<Grocery, GroceryId>
{
    Task<IReadOnlyList<Grocery>> SearchByNameAsync(string nameFragment, CancellationToken cancellationToken = default);
    Task<Grocery?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
}
