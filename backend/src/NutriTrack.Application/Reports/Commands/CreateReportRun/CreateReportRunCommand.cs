using ErrorOr;
using MediatR;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Reports;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Reports.Commands.CreateReportRun;

public sealed record CreateReportRunCommand(
    UserId RequestedBy,
    ReportType Type,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    object? Parameters) : IRequest<ErrorOr<ReportRunResult>>;
