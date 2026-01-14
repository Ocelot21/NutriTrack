using NutriTrack.Domain.Common;
using NutriTrack.Domain.Common.Errors;
using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Countries;

public sealed class CountryCode : ValueObject
{
    public string Value { get; }

    private CountryCode(string value)
    {
        Value = value;
    }

    public static CountryCode? CreateOptional(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return Create(raw);
    }

    public static CountryCode Create(string raw)
    {
        var value = raw.Trim().ToUpperInvariant();

        if (!DomainPatterns.CountryIso2Regex.IsMatch(value))
        {
            throw new DomainException(
                DomainErrorCodes.Users.InvalidCountry,
                "Country must be ISO-3166 alpha-2 (e.g., BA, HR, US).");
        }

        return new CountryCode(value);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}