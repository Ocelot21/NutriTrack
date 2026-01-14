using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Reports.Queries.ListReportRuns;

public sealed record ListReportRunsQuery(
    UserId RequestedBy,
    int Page,
    int PageSize) : IRequest<ErrorOr<PagedResult<ReportRunResult>>>;
