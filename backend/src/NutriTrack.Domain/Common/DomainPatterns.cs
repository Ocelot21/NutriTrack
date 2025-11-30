using System.Text.RegularExpressions;

namespace NutriTrack.Domain.Common;

public static class DomainPatterns
{
    // Basic email pattern (intentionally simple; full RFC patterns are excessive for most apps).
    public const string BasicEmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    // Username: starts with letter; then letters, digits, underscore, dot; length enforced separately.
    public const string UsernamePattern = @"^[a-zA-Z][a-zA-Z0-9_.]{2,31}$";

    // Country ISO-3166 alpha-2: two uppercase letters.
    public const string CountryIso2Pattern = @"^[A-Z]{2}$";

    // Permission key: lowercase letters, digits, colon and dot separators (e.g. users.read, roles:assign).
    public const string PermissionKeyPattern = @"^[a-zA-Z0-9]+([.:][a-zA-Z0-9]+)*$";

    public static readonly Regex UsernameRegex = new(UsernamePattern, RegexOptions.Compiled);
    public static readonly Regex EmailRegex = new(BasicEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly Regex CountryIso2Regex = new(CountryIso2Pattern, RegexOptions.Compiled);
    public static readonly Regex PermissionKeyRegex = new(PermissionKeyPattern, RegexOptions.Compiled);
}