using FluentValidation;

namespace NutriTrack.Application.Reports.Commands.CreateReportRun;

public sealed class CreateReportRunCommandValidator : AbstractValidator<CreateReportRunCommand>
{
    public CreateReportRunCommandValidator()
    {
        RuleFor(x => x.RequestedBy.Value).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.FromUtc).NotEmpty();
        RuleFor(x => x.ToUtc).NotEmpty();
        RuleFor(x => x.ToUtc).GreaterThan(x => x.FromUtc);
    }
}
