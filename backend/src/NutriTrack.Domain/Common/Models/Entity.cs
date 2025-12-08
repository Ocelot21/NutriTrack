using NutriTrack.Domain.Common.Events;

namespace NutriTrack.Domain.Common.Models;

public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;


    protected Entity()
    {
        // EF Core
    }

    protected Entity(TId id)
    {
        Id = id;
    }
}
