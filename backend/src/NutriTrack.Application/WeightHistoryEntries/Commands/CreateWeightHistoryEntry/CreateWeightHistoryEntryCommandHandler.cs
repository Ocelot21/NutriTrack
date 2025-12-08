using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.WeightHistory;

namespace NutriTrack.Application.WeightHistoryEntries.Commands.CreateWeightHistoryEntry;

public sealed class CreateWeightHistoryEntryCommandHandler : IRequestHandler<CreateWeightHistoryEntryCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IWeightHistoryRepository _weightHistoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateWeightHistoryEntryCommandHandler(
        IUserRepository userRepository,
        IWeightHistoryRepository weightHistoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _weightHistoryRepository = weightHistoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<Unit>> Handle(CreateWeightHistoryEntryCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Errors.Users.NotFound;
        }

        var utcNow = _dateTimeProvider.UtcNow;
        var entry = WeightHistoryEntry.Create(request.UserId, request.Date, request.WeightKg, utcNow);

        await _weightHistoryRepository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}