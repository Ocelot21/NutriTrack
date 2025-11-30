using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Groceries;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class GroceryRepository : EfRepository<Grocery, GroceryId>, IGroceryRepository
{
    public GroceryRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Grocery>> SearchByNameAsync(string nameFragment, CancellationToken cancellationToken = default)
        => await _dbContext.Groceries
            .Where(g => g.Name.Contains(nameFragment))
            .ToListAsync(cancellationToken);

    public async Task<Grocery?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        => await _dbContext.Groceries.FirstOrDefaultAsync(g => g.Barcode == barcode, cancellationToken);
}
