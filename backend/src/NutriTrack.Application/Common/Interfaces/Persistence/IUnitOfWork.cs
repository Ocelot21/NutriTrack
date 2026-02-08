using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Application.Common.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void MarkAsModified<TEntity>(TEntity entity) where TEntity : class;
    }
}
