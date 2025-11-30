using NutriTrack.Domain.Common;
using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Users;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string raw)
    {
        if (raw is null)
        {
            throw new DomainException(DomainErrorCodes.Users.InvalidEmail, "Email cannot be null.");
        }

        var value = raw.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(DomainErrorCodes.Users.InvalidEmail, "Email cannot be empty.");
        }

        if (value.Length > DomainConstraints.Users.MaxEmailLength)
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidEmail,
                $"Email cannot be longer than {DomainConstraints.Users.MaxEmailLength} characters.");
        }

        if (!DomainPatterns.EmailRegex.IsMatch(value))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidEmail,
                "Email format is invalid.");
        }

        return new Email(value.ToLowerInvariant());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
