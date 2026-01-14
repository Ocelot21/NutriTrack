using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Security;
using NutriTrack.Application.TwoFactor.Common;
using NutriTrack.Domain.Users;
using OtpNet;

namespace NutriTrack.Application.TwoFactor.Commands.SetupTotp;

public sealed class SetupTotpCommandHandler : IRequestHandler<SetupTotpCommand, ErrorOr<TotpSetupResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPendingTotpSetupRepository _pendingTotpSetupRepository;
    private readonly ITotpSecretProtector _totpSecretProtector;
    private readonly IUnitOfWork _unitOfWork;

    public SetupTotpCommandHandler(
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

    public async Task<ErrorOr<TotpSetupResult>> Handle(SetupTotpCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            is not User user)
        {
            return Errors.Users.NotFound;
        }

        var secretBytes = KeyGeneration.GenerateRandomKey(20);
        var manualBase32 = Base32Encoding.ToString(secretBytes);

        var secretProtected = _totpSecretProtector.Protect(manualBase32);

        var pending = new PendingTotpSetup(request.UserId, secretProtected, DateTime.UtcNow.AddMinutes(10));

        await _pendingTotpSetupRepository.UpsertAsync(pending, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var issuer = "NutriTrack";
        var label = user.Email.Value;

        var uri = new OtpUri(OtpType.Totp, manualBase32, label, issuer).ToString();

        return new TotpSetupResult(uri, manualBase32);
    }
}
