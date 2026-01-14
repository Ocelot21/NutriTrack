using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Models;

namespace NutriTrack.Application.Users.Queries.ListUsers;

public record ListUsersQuery(int PageNumber, int PageSize) : IRequest<ErrorOr<PagedResult<UserResult>>>;
