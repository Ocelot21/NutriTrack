using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Domain.Common.Models;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity
    where TId : notnull
{
    public DateTime CreatedAtUtc { get; protected set; }
    public UserId? CreatedBy { get; protected set; }

    public DateTime? ModifiedAtUtc { get; protected set; }
    public UserId? ModifiedBy { get; protected set; }

    protected AuditableEntity() : base()
    {
    }

    protected AuditableEntity(TId id) : base(id)
    {
    }

    public void SetCreated(DateTime utcNow, UserId? userId)
    {
        CreatedAtUtc = utcNow;
        CreatedBy = userId;
    }

    public void SetModified(DateTime utcNow, UserId? userId)
    {
        ModifiedAtUtc = utcNow;
        ModifiedBy = userId;
    }
}
