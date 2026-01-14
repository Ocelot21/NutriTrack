using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.Application.Common.Interfaces.Storage;
using NutriTrack.Application.Common.Storage;
using NutriTrack.Application.Reports.Commands.CreateReportRun;
using NutriTrack.Application.Reports.Queries.GetReportRunById;
using NutriTrack.Application.Reports.Queries.ListReportRuns;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Reports;
using NutriTrack.Domain.Authorization;
using NutriTrack.Domain.Reports;

namespace NutriTrack.Api.Controllers;

[Route("api/admin/reports")]
public sealed class ReportsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorage;

    public ReportsController(ISender mediator, IMapper mapper, IBlobStorageService blobStorage)
    {
        _mediator = mediator;
        _mapper = mapper;
        _blobStorage = blobStorage;
    }

    [Authorize(Policy = PermissionKeys.Reports.Read)]
    [HttpGet("runs")]
    public async Task<IActionResult> ListRuns(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new ListReportRunsQuery(userId, page, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            paged => Ok(new ListReportRunsResponse(
                new PagedResponse<ReportRunResponse>(
                    paged.Items.Select(Map).ToList(),
                    paged.TotalCount,
                    paged.Page,
                    paged.PageSize))),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Reports.Read)]
    [HttpGet("runs/{id:guid}")]
    public async Task<IActionResult> GetRun(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new GetReportRunByIdQuery(new ReportRunId(id), userId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            run => Ok(Map(run)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Reports.Create)]
    [HttpPost("runs")]
    public async Task<IActionResult> CreateRun(
        [FromBody] CreateReportRunRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();

        var command = new CreateReportRunCommand(
            RequestedBy: userId,
            Type: (ReportType)request.Type,
            FromUtc: request.FromUtc,
            ToUtc: request.ToUtc,
            Parameters: request.Parameters);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match(
            run => CreatedAtAction(nameof(GetRun), new { id = run.Id.Value }, Map(run)),
            errors => Problem(errors));
    }

    [Authorize(Policy = PermissionKeys.Reports.Read)]
    [HttpGet("runs/{id:guid}/pdf")]
    public async Task<IActionResult> DownloadPdf(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var query = new GetReportRunByIdQuery(new ReportRunId(id), userId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match(
            run =>
            {
                var uri = !string.IsNullOrWhiteSpace(run.OutputPdfBlobName)
                    ? _blobStorage.GenerateReadUri(BlobContainer.Reports, run.OutputPdfBlobName)
                    : run.OutputPdfUri;

                if (string.IsNullOrWhiteSpace(uri))
                {
                    return NotFound();
                }

                return Redirect(uri);
            },
            errors => Problem(errors));
    }

    private static ReportRunResponse Map(Application.Reports.Common.ReportRunResult run)
        => new(
            run.Id.Value,
            (int)run.Type,
            (int)run.Status,
            run.RequestedAtUtc,
            run.FromUtc,
            run.ToUtc,
            run.OutputPdfUri,
            run.OutputPdfBlobName,
            run.OutputFileName,
            run.StartedAtUtc,
            run.CompletedAtUtc,
            run.FailureReason);
}
