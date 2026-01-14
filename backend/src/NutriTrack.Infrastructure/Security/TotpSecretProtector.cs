using Microsoft.AspNetCore.DataProtection;
using NutriTrack.Application.Common.Interfaces.Security;

namespace NutriTrack.Infrastructure.Security;

public sealed class TotpSecretProtector : ITotpSecretProtector
{
    private readonly IDataProtector _protector;

    public TotpSecretProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("NutriTrack.TotpSecret.v1");
    }

    public string Protect(string secretPlain) => _protector.Protect(secretPlain);
    public string Unprotect(string secretProtected) => _protector.Unprotect(secretProtected);
}
