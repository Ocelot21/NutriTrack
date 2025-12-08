using Microsoft.EntityFrameworkCore;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infrastructure.Persistence.Repositories;

public sealed class AchievementRepository : EfRepository<Achievement, AchievementId>, IAchievementRepository
{
    public AchievementRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<Achievement?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {

        return await _dbContext.Achievements.FirstOrDefaultAsync(a => a.Key == key, cancellationToken);
    }
}