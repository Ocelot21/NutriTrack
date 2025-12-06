using NutriTrack.Notifications.Domain.Users;

namespace NutriTrack.Notifications.Domain.Common.Models;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }
    UserId? CreatedBy { get; }

    DateTime? ModifiedAtUtc { get; }
    UserId? ModifiedBy { get; }

    void SetCreated(DateTime utcNow, UserId? userId);
    void SetModified(DateTime utcNow, UserId? userId);
}
