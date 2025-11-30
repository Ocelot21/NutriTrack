using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Common;

namespace NutriTrack.Domain.Authorization;

public sealed class Role : AggregateRoot<RoleId>
{
    [Obsolete("Constructor for EF Core only", error: false)]
    private Role() : base()
    {
        // Parameterless constructor required by EF Core
    }

    private Role(
        RoleId id,
        string name,
        string? description,
        bool isSystemRole,
        bool isActive)
        : base(id)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
        IsActive = isActive;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public bool IsSystemRole { get; private set; }
    public bool IsActive { get; private set; }

    public static Role Create(
        string name,
        string? description = null,
        bool isSystemRole = false)
    {
        name = NormalizeName(name);
        description = NormalizeDescription(description);

        return new Role(
            new RoleId(Guid.NewGuid()),
            name,
            description,
            isSystemRole,
            isActive: true);
    }

    public void UpdateDetails(
        Optional<string> name,
        Optional<string?> description)
    {
        if (name.IsSet)
        {
            Name = NormalizeName(name.Value);
        }

        if (description.IsSet)
        {
            Description = NormalizeDescription(description.Value);
        }
    }

    public void Deactivate()
    {
        if (!IsSystemRole)
        {
            IsActive = false;
        }
        // TODO: consider throwing a domain exception if system roles must never be deactivated.
    }

    private static string NormalizeName(string name)
    {
        var value = name.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidRoleName,
                "Role name cannot be empty.");
        }

        if (value.Length > DomainConstraints.Authorization.MaxRoleNameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidRoleName,
                $"Role name cannot be longer than {DomainConstraints.Authorization.MaxRoleNameLength} characters.");
        }

        return value;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var value = description.Trim();

        if (value.Length > DomainConstraints.Authorization.MaxRoleDescriptionLength)
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidRoleDescription,
                $"Role description cannot be longer than {DomainConstraints.Authorization.MaxRoleDescriptionLength} characters.");
        }

        return value;
    }

    public override string ToString() => Name;
}
