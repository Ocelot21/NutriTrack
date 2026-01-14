using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Groceries;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Groceries.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class GroceryRepository : EfRepository<Grocery, GroceryId>, IGroceryRepository
{
    public GroceryRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Grocery>> SearchByNameAsync(
        string nameFragment,
        CancellationToken cancellationToken = default)
        => await _dbContext.Groceries
            .Where(g => g.Name.Contains(nameFragment))
            .ToListAsync(cancellationToken);

    public async Task<Grocery?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        => await _dbContext.Groceries.FirstOrDefaultAsync(g => g.Barcode == barcode, cancellationToken);

    public async Task<PagedResult<Grocery>> GetPagedAsync(
        GroceryListFilters filters,
        UserId? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext
            .Groceries
            .AsQueryable();

        query = query.Where(g => !g.IsDeleted);

        if (userId is null)
        {
            query = query.Where(g => g.IsApproved);
        }
        else
        {
            var currentUserId = userId.Value;

            query = query.Where(g =>
                g.IsApproved ||
                (g.CreatedBy != null && g.CreatedBy == currentUserId));
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var term = filters.SearchTerm.Trim().ToLower();

            query = query.Where(g =>
                g.Name.ToLower().Contains(term) ||
                (g.Barcode != null && g.Barcode.ToLower().Contains(term)));
        }

        if (filters.Category.HasValue)
        {
            query = query.Where(g => g.Category == filters.Category.Value);
        }

        if (filters.UnitOfMeasure.HasValue)
        {
            query = query.Where(g => g.UnitOfMeasure == filters.UnitOfMeasure.Value);
        }

        if (filters.MinCaloriesPer100.HasValue)
        {
            query = query.Where(g =>
                g.CaloriesPer100 >= filters.MinCaloriesPer100.Value);
        }

        if (filters.MaxCaloriesPer100.HasValue)
        {
            query = query.Where(g =>
                g.CaloriesPer100 <= filters.MaxCaloriesPer100.Value);
        }

        if (filters.MinProteinPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.ProteinGramsPer100 >= filters.MinProteinPer100g.Value);
        }

        if (filters.MaxProteinPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.ProteinGramsPer100 <= filters.MaxProteinPer100g.Value);
        }

        if (filters.MinCarbsPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.CarbsGramsPer100 >= filters.MinCarbsPer100g.Value);
        }

        if (filters.MaxCarbsPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.CarbsGramsPer100 <= filters.MaxCarbsPer100g.Value);
        }

        if (filters.MinFatPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.FatGramsPer100 >= filters.MinFatPer100g.Value);
        }

        if (filters.MaxFatPer100g.HasValue)
        {
            query = query.Where(g =>
                g.MacrosPer100.FatGramsPer100 <= filters.MaxFatPer100g.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Grocery>(
            items,
            totalCount,
            page,
            pageSize);
    }

    public async Task<PagedResult<Grocery>> GetPagedByApprovalAsync(bool isApproved, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Groceries.AsQueryable()
            .Where(g => g.IsApproved == isApproved && !g.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(g => g.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Grocery>(items, totalCount, page, pageSize);
    }
}
