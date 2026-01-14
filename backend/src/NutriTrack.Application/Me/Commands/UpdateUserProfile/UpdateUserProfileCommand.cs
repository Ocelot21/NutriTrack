using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    UserId UserId,
    string? Email,
    string? Username,
    string? FirstName,
    string? LastName,
    string? TimeZoneId,
    string? CountryIso2
) : IRequest<ErrorOr<Unit>>;
