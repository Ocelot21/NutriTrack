using ErrorOr;
using MediatR;
using NutriTrack.Application.Authentication.Common;

namespace NutriTrack.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;
