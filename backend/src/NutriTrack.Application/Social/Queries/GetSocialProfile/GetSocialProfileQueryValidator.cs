using FluentValidation;

namespace NutriTrack.Application.Social.Queries.GetSocialProfile;

public sealed class GetSocialProfileQueryValidator : AbstractValidator<GetSocialProfileQuery>
{
    public GetSocialProfileQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
