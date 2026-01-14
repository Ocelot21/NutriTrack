using ErrorOr;
using MediatR;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Reports.Queries.GetReportRunById;

public sealed record GetReportRunByIdQuery(
    ReportRunId Id,
    UserId RequestedBy) : IRequest<ErrorOr<ReportRunResult>>;
