using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Exercises.Common;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Exercises.Queries.GetExerciseById;

public sealed class GetExerciseByIdQueryHandler : IRequestHandler<GetExerciseByIdQuery, ErrorOr<ExerciseResult>>
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetExerciseByIdQueryHandler(IExerciseRepository exerciseRepository, IBlobStorageService blobStorageService)
    {
        _exerciseRepository = exerciseRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<ExerciseResult>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return Errors.Exercises.NotFound;
        }

        var result = entity.ToExerciseResult();
        return result with
        {
            ImageUrl = _blobStorageService.GenerateReadUri(BlobContainer.Exercises, result.ImageUrl)
        };
    }
}
