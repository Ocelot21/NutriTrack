using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;

namespace NutriTrack.Application.Reports.Queries.ListReportRuns;

public sealed class ListReportRunsQueryHandler
    : IRequestHandler<ListReportRunsQuery, ErrorOr<PagedResult<ReportRunResult>>>
{
    private readonly IReportRunRepository _repo;
    private readonly IBlobStorageService _blobStorageService;

    public ListReportRunsQueryHandler(IReportRunRepository repo, IBlobStorageService blobStorageService)
    {
        _repo = repo;
        _blobStorageService = blobStorageService;
    }

    public async Task<ErrorOr<PagedResult<ReportRunResult>>> Handle(ListReportRunsQuery request, CancellationToken cancellationToken)
    {
        var paged = await _repo.ListForUserAsync(
            requestedBy: request.RequestedBy,
            page: request.Page,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var items = paged.Items
            .Select(x => x.ToReportRunResult())
            .Select(r => r with { OutputPdfUri = _blobStorageService.GenerateReadUri(BlobContainer.Reports, r.OutputPdfBlobName) })
            .ToList();

        return new PagedResult<ReportRunResult>(items, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
