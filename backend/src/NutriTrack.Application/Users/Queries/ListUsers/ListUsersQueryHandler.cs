using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Common.Mappings;
using NutriTrack.Application.Common.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriTrack.Application.Users.Queries.ListUsers
{
    public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, ErrorOr<PagedResult<UserResult>>>
    {
        private readonly IUserRepository _userRepository;

        public ListUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<PagedResult<UserResult>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var page = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            var paged = await _userRepository.GetPagedAsync(page, pageSize, cancellationToken);

            var results = paged.Items
                .Select(u => u.ToUserResult())
                .ToList();

            return new PagedResult<UserResult>(results, paged.TotalCount, paged.Page, paged.PageSize);
        }
    }
}
