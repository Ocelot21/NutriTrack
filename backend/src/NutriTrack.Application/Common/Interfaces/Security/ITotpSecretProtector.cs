namespace NutriTrack.Application.Common.Interfaces.Security;

public interface ITotpSecretProtector
{
    string Protect(string secretPlain);
    string Unprotect(string secretProtected);
}
