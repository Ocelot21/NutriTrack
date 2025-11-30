using NutriTrack.Domain.Users;

namespace NutriTrack.Domain.Common.Models
{
    public interface IAuditableEntity
    {
        DateTime CreatedAtUtc { get; }
        UserId? CreatedBy { get; }
        DateTime? ModifiedAtUtc { get; }
        UserId? ModifiedBy { get; }
        void SetCreated(DateTime utcNow, UserId? userId);
        void SetModified(DateTime utcNow, UserId? userId);
    }
}
