using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class ExerciseRepository : EfRepository<Exercise, ExerciseId>, IExerciseRepository
{
    public ExerciseRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Exercise>> GetApprovedAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Exercises
            .Where(e => e.IsApproved && !e.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<PagedResult<Exercise>> GetPagedAsync(
       ExerciseListFilters filters,
       UserId? userId,
       int page,
       int pageSize,
       CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext
            .Exercises
            .AsQueryable();

        if (userId is null)
        {
            query = query.Where(e => e.IsApproved);
        }
        else
        {
            var currentUserId = userId.Value;
            query = query.Where(e =>
                e.IsApproved || (e.CreatedBy != null && e.CreatedBy == currentUserId));
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var term = filters.SearchTerm.Trim().ToLower();

            query = query.Where(e =>
                e.Name.ToLower().Contains(term) ||
                (e.Description != null && e.Description.ToLower().Contains(term)));
        }

        if (filters.Category.HasValue)
        {
            query = query.Where(e => e.Category == filters.Category.Value);
        }

        if (filters.MinCaloriesPerMinute.HasValue)
        {
            query = query.Where(e =>
                e.DefaultCaloriesPerMinute >= filters.MinCaloriesPerMinute.Value);
        }

        if (filters.MaxCaloriesPerMinute.HasValue)
        {
            query = query.Where(e =>
                e.DefaultCaloriesPerMinute <= filters.MaxCaloriesPerMinute.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Exercise>(
            items,
            totalCount,
            page,
            pageSize);
    }

    public async Task<PagedResult<Exercise>> GetPagedByApprovalAsync(bool isApproved, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Exercises.AsQueryable()
            .Where(e => e.IsApproved == isApproved && !e.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Exercise>(items, totalCount, page, pageSize);
    }
}
