using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Common.Primitives;

namespace NutriTrack.Application.UserExercises.Commands.UpdateUserExerciseLog;

public sealed class UpdateUserExerciseLogCommandHandler : IRequestHandler<UpdateUserExerciseLogCommand, ErrorOr<Unit>>
{
    private readonly IUserExerciseLogRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserExerciseLogCommandHandler(IUserExerciseLogRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateUserExerciseLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        entity.Update(
            request.DurationMinutes.HasValue ? request.DurationMinutes.Value : Optional<decimal>.None(),
            request.OccurredAtUtc,
            request.OccurredAtLocal,
            request.LocalDate,
            request.Notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}