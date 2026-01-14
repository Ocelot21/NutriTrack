using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Domain.Users;
using OtpNet;

namespace NutriTrack.Application.TwoFactor.Queries.VerifyTotp;

public sealed class VerifyTotpQueryHandler : IRequestHandler<VerifyTotpQuery, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpSecretProtector _totpSecretProtector;

    public VerifyTotpQueryHandler(IUserRepository userRepository, ITotpSecretProtector totpSecretProtector)
    {
        _userRepository = userRepository;
        _totpSecretProtector = totpSecretProtector;
    }

    public async Task<ErrorOr<Success>> Handle(VerifyTotpQuery request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken) 
            is not User user)
        {
            return Errors.Users.NotFound;
        }

        if (!user.IsTwoFactorEnabled || string.IsNullOrWhiteSpace(user.TotpSecretProtected))
            return Error.Conflict("Totp.NotEnabled", "TOTP is not enabled for this user.");

        var secretBase32 = _totpSecretProtector.Unprotect(user.TotpSecretProtected);
        var secretBytes = Base32Encoding.ToBytes(secretBase32);

        var totp = new Totp(secretBytes, step: 30, totpSize: 6);
        var ok = totp.VerifyTotp(request.Code, out _, new VerificationWindow(previous: 1, future: 1));

        return ok
            ? Result.Success
            : Error.Validation("Totp.Invalid", "Code is invalid.");
    }
}