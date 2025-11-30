namespace NutriTrack.Domain.Common.Models;

public abstract class AggregateRoot<TId> : AuditableEntity<TId>, IAggregateRoot
    where TId : notnull
{
    protected AggregateRoot()
        : base()
    {
    }

    protected AggregateRoot(TId id)
        : base(id)
    {
    }
}
