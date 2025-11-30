using FluentValidation;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Queries.GetMe;

public sealed class GetMeQueryValidator : AbstractValidator<GetMeQuery>
{
    public GetMeQueryValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id.Value != Guid.Empty)
            .WithMessage("User id is required.");
    }
}
