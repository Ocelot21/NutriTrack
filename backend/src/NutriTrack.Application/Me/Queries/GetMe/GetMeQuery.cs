using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;
using NutriTrack.Domain.Users;

namespace NutriTrack.Application.Me.Queries.GetMe
{
    public record GetMeQuery(UserId Id) : IRequest<ErrorOr<UserResult>>;
}