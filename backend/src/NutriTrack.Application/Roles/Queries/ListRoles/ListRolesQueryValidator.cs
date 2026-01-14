using FluentValidation;

namespace NutriTrack.Application.Roles.Queries.ListRoles;

public sealed class ListRolesQueryValidator : AbstractValidator<ListRolesQuery>
{
    public ListRolesQueryValidator()
    {
    }
}
