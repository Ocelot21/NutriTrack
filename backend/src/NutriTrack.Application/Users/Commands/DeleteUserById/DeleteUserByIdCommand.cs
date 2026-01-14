using ErrorOr;
using MediatR;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Users.Commands.DeleteUserById;

public sealed record DeleteUserByIdCommand(UserId UserId) : IRequest<ErrorOr<Unit>>;
