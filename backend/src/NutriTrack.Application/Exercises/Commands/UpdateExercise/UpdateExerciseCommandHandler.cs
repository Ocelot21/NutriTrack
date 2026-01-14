using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.UpdateExercise;

public sealed class UpdateExerciseCommandHandler : IRequestHandler<UpdateExerciseCommand, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public UpdateExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        if (request.Image is not null)
        {
            var oldBlobName = entity.ImageUrl;
            var newBlobName = await _blobStorageService.UploadAsync(
                BlobContainer.Exercises,
                request.Image,
                request.ImageFileName ?? "image",
                request.ImageContentType ?? "application/octet-stream",
                cancellationToken);

            entity.SetImage(newBlobName);

            await _blobStorageService.DeleteAsync(BlobContainer.Exercises, oldBlobName, cancellationToken);
        }

        entity.UpdateDefinition(
            request.Name is null ? Optional<string>.None() : request.Name,
            request.Description is null ? Optional<string>.None() : request.Description,
            request.Category is null ? Optional<ExerciseCategory>.None() : request.Category.Value,
            request.DefaultCaloriesPerMinute is null ? Optional<decimal>.None() : request.DefaultCaloriesPerMinute.Value);

        if (request.IsApproved.HasValue && request.IsApproved.Value)
        {
            entity.Approve();
        }

        if (request.IsDeleted.HasValue && request.IsDeleted.Value)
        {
            entity.MarkDeleted();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.ToExerciseResult();
    }
}
