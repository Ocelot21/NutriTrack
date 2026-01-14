using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.DeleteExercise;

public sealed class DeleteExerciseCommandHandler : IRequestHandler<DeleteExerciseCommand, ErrorOr<Unit>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<Unit>> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        var blobName = entity.ImageUrl;
        _exerciseRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _blobStorageService.DeleteAsync(BlobContainer.Exercises, blobName, cancellationToken);
        return Unit.Value;
    }
}
