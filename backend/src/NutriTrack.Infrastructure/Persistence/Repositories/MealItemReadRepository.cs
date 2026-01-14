using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Persistence;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class MealItemReadRepository : IMealItemReadRepository
{
    private readonly AppDbContext _dbContext;

    public MealItemReadRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<Guid, int>> CountByGroceryForUsersAsync(
        IReadOnlyCollection<UserId> userIds,
        CancellationToken cancellationToken = default)
    {
        if (userIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        var userGuids = userIds.Select(u => u.Value).ToArray();

        var sql = @"
        SELECT mi.GroceryId, COUNT(1) AS [Count]
        FROM MealItem mi
        INNER JOIN Meals m ON mi.MealId = m.Id
        WHERE m.UserId IN ({0})
        GROUP BY mi.GroceryId";

        var parameters = new List<object>();
        var placeholders = new List<string>();
        for (var i = 0; i < userGuids.Length; i++)
        {
            placeholders.Add($"{{{i}}}");
            parameters.Add(userGuids[i]);
        }

        var finalSql = string.Format(sql, string.Join(",", placeholders));

        var results = await _dbContext.Database
            .SqlQueryRaw<MealItemCountRow>(finalSql, parameters.ToArray())
            .ToListAsync(cancellationToken);

        return results.ToDictionary(r => r.GroceryId, r => r.Count);
    }

    private sealed record MealItemCountRow(Guid GroceryId, int Count);
}
