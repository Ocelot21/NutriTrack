using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Reports.Common;
using NutriTrack.Domain.Reports;

namespace NutriTrack.Application.Reports.Commands.CreateReportRun;

public sealed class CreateReportRunCommandHandler : IRequestHandler<CreateReportRunCommand, ErrorOr<ReportRunResult>>
{
    private readonly IReportRunRepository _reportRunRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateReportRunCommandHandler(
        IReportRunRepository reportRunRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _reportRunRepository = reportRunRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ReportRunResult>> Handle(CreateReportRunCommand request, CancellationToken cancellationToken)
    {
        var utcNow = _dateTimeProvider.UtcNow;

        var run = ReportRun.Create(
            id: new ReportRunId(Guid.NewGuid()),
            type: request.Type,
            requestedBy: request.RequestedBy,
            requestedAtUtc: utcNow,
            fromUtc: request.FromUtc,
            toUtc: request.ToUtc,
            parameters: request.Parameters);

        await _reportRunRepository.AddAsync(run, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return run.ToReportRunResult();
    }
}
