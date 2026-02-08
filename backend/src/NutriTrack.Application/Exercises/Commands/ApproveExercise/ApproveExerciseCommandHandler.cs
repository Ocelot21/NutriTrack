using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;

namespace NutriTrack.Application.Exercises.Commands.ApproveExercise;

public sealed class ApproveExerciseCommandHandler
    : IRequestHandler<ApproveExerciseCommand, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public ApproveExerciseCommandHandler(
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(ApproveExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (exercise is null)
        {
            return Errors.Exercises.NotFound;
        }

        exercise.Approve();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = exercise.ToExerciseResult();
        return result with
        {
            ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Exercises, result.ImageUrl)
        };
    }
}
