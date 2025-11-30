using NutriTrack.Domain.Common;
using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Authorization;

public sealed class PermissionKey : ValueObject
{
    public string Value { get; }

    private PermissionKey(string value)
    {
        Value = value;
    }

    public static PermissionKey Create(string raw)
    {
        if (raw is null)
        {
            throw new DomainException(DomainErrorCodes.Authorization.InvalidPermissionKey, "Permission key cannot be null.");
        }

        var value = raw.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(DomainErrorCodes.Authorization.InvalidPermissionKey, "Permission key cannot be empty.");
        }

        if (value.Length > DomainConstraints.Authorization.MaxPermissionKeyLength)
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidPermissionKey,
                $"Permission key cannot be longer than {DomainConstraints.Authorization.MaxPermissionKeyLength} characters.");
        }

        if (!DomainPatterns.PermissionKeyRegex.IsMatch(value))
        {
            throw new DomainException(
                DomainErrorCodes.Authorization.InvalidPermissionKey,
                "Permission key format is invalid.");
        }

        return new PermissionKey(value);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
