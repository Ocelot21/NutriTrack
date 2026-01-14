using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Errors;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Reports.Queries.GetReportRunById;

public sealed class GetReportRunByIdQueryHandler : IRequestHandler<GetReportRunByIdQuery, ErrorOr<ReportRunResult>>
{
    private readonly IReportRunRepository _repo;
    private readonly IBlobStorageService _blobStorageService;

    public GetReportRunByIdQueryHandler(IReportRunRepository repo, IBlobStorageService blobStorageService)
    {
        _repo = repo;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<ReportRunResult>> Handle(GetReportRunByIdQuery request, CancellationToken cancellationToken)
    {
        var run = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (run is null)
        {
            return Error.NotFound(code: "Reports.NotFound", description: "Report run was not found.");
        }

        if (run.RequestedBy != request.RequestedBy)
        {
            return Errors.Authorization.Unauthorized;
        }

        var result = run.ToReportRunResult();
        return result with
        {
            OutputPdfUri = _blobStorageService.GenerateReadUri(BlobContainer.Reports, result.OutputPdfBlobName)
        };
    }
}
