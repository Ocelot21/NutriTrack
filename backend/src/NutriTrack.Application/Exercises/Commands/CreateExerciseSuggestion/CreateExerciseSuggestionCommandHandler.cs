using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Domain.Common.Primitives;
using NutriTrack.Domain.Exercises;

namespace NutriTrack.Application.Exercises.Commands.CreateExerciseSuggestion;

public sealed class CreateExerciseSuggestionCommandHandler
    : IRequestHandler<CreateExerciseSuggestionCommand, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public CreateExerciseSuggestionCommandHandler(
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(
        CreateExerciseSuggestionCommand request,
        CancellationToken cancellationToken)
    {
        var descriptionOpt = request.Description is null
            ? Optional<string>.None()
            : request.Description;

        string? imageBlobName = null;
        if (request.Image is not null)
        {
            imageBlobName = await _blobStorageService.UploadAsync(
                BlobContainer.Exercises,
                request.Image,
                request.ImageFileName ?? "image",
                request.ImageContentType ?? "application/octet-stream",
                cancellationToken);
        }

        var entity = Exercise.Create(
            request.Name,
            descriptionOpt,
            request.Category,
            request.DefaultCaloriesPerMinute,
            imageBlobName,
            isApproved: false);

        await _exerciseRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = entity.ToExerciseResult();
        return result with
        {
            ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Exercises, result.ImageUrl)
        };
    }
}
