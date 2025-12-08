using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.UserGoals;

namespace NutriTrack.Application.UserGoals.Commands.CancelUserGoal;

public sealed class CancelUserGoalCommandHandler : IRequestHandler<CancelUserGoalCommand, ErrorOr<Unit>>
{
    private readonly IUserGoalRepository _userGoalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelUserGoalCommandHandler(
        IUserGoalRepository userGoalRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userGoalRepository = userGoalRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<Unit>> Handle(CancelUserGoalCommand request, CancellationToken cancellationToken)
    {
        var goal = await _userGoalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (goal is null)
        {
            return Errors.UserGoals.NotFound;
        }

        goal.Cancel(_dateTimeProvider.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
