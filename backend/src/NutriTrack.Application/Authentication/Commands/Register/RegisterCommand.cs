using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;

namespace NutriTrack.Application.Authentication.Commands.Register
{
    public record RegisterCommand(
        string FirstName, 
        string LastName,
        string Username,
        string Email,
        string Password,
        string ConfirmPassword,
        string CountryIso2,
        string TimeZoneId) : IRequest<ErrorOr<AuthenticationResult>>;
}