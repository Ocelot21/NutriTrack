using NutriTrack.Domain.Common;
using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Users;

public sealed class Username : ValueObject
{
    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Username Create(string raw)
    {
        if (raw is null)
        {
            throw new DomainException(DomainErrorCodes.Users.InvalidUsername, "Username cannot be null.");
        }

        var value = raw.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(DomainErrorCodes.Users.InvalidUsername, "Username cannot be empty.");
        }

        if (value.Length < DomainConstraints.Users.MinUsernameLength || value.Length > DomainConstraints.Users.MaxUsernameLength)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidUsername,
                $"Username must be between {DomainConstraints.Users.MinUsernameLength} and {DomainConstraints.Users.MaxUsernameLength} characters.");
        }

        if (!DomainPatterns.UsernameRegex.IsMatch(value))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidUsername,
                "Username format is invalid.");
        }

        return new Username(value);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
