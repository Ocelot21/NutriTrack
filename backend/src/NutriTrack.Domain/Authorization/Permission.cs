using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Authorization;

public sealed class Permission : AggregateRoot<PermissionId>
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private Permission() : base()
    {
        // Parameterless constructor required by EF Core
    }

    private Permission(
        PermissionId id,
        PermissionKey key,
        string? description)
        : base(id)
    {
        Key = key;
        Description = description;
    }

    public PermissionKey Key { get; private set; } = null!;
    public string? Description { get; private set; }

    public static Permission Create(
        string key,
        string? description = null)
    {
        var pk = PermissionKey.Create(key);
        var desc = NormalizeDescription(description);

        return new Permission(
            new PermissionId(Guid.NewGuid()),
            pk,
            desc);
    }

    public void Update(
        Optional<string> key,
        Optional<string?> description)
    {
        if (key.IsSet)
        {
            Key = PermissionKey.Create(key.Value);
        }

        if (description.IsSet)
        {
            Description = NormalizeDescription(description.Value);
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var value = description.Trim();

        if (value.Length > DomainConstraints.Authorization.MaxPermissionDescriptionLength)
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidPermissionDescription,
                $"Permission description cannot be longer than {DomainConstraints.Authorization.MaxPermissionDescriptionLength} characters.");
        }

        return value;
    }
}
