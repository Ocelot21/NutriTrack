using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(UserId Id) : IRequest<ErrorOr<UserResult>>;
