using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Domain.Users;
using OtpNet;

namespace NutriTrack.Application.TwoFactor.Commands.ConfirmTotp;

public sealed class ConfirmTotpCommandHandler : IRequestHandler<ConfirmTotpCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPendingTotpSetupRepository _pendingTotpSetupRepository;
    private readonly ITotpSecretProtector _totpSecretProtector;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmTotpCommandHandler(
        IUserRepository userRepository,
        IPendingTotpSetupRepository pendingTotpSetupRepository,
        ITotpSecretProtector totpSecretProtector,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _pendingTotpSetupRepository = pendingTotpSetupRepository;
        _totpSecretProtector = totpSecretProtector;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmTotpCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            is not User user)
        {
            return Errors.Users.NotFound;
        }

        var pending = await _pendingTotpSetupRepository.GetAsync(request.UserId, cancellationToken);
        if (pending is null || pending.ExpiresAtUtc <= DateTime.UtcNow)
            return Error.Validation("Totp.SetupExpired", "TOTP setup expired. Please setup again.");

        var secretBase32 = _totpSecretProtector.Unprotect(pending.SecretProtected);
        var secretBytes = Base32Encoding.ToBytes(secretBase32);

        var totp = new Totp(secretBytes, step: 30, totpSize: 6);
        // allow small time drift: -1..+1 step
        var ok = totp.VerifyTotp(request.Code, out _, new VerificationWindow(previous: 1, future: 1));

        if (!ok)
            return Error.Validation("Totp.Invalid", "Code is invalid.");

        user.EnableTotp(pending.SecretProtected);
        await _pendingTotpSetupRepository.DeleteAsync(request.UserId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
