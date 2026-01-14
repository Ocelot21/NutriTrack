using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Services;

namespace NutriTrack.Application.Metadata.Queries.GetSystemTimeZones
{
    public class GetSystemTimeZonesQueryHandler : IRequestHandler<GetSystemTimeZonesQuery, ErrorOr<IReadOnlyList<string>>>
    {
        private readonly ITimeZoneService _timeZoneService;

        public GetSystemTimeZonesQueryHandler(ITimeZoneService timeZoneService)
        {
            _timeZoneService = timeZoneService;
        }

        public async Task<ErrorOr<IReadOnlyList<string>>> Handle(
            GetSystemTimeZonesQuery request,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var timeZones = _timeZoneService.GetSystemTimeZoneIds();

            return timeZones.ToList();
        }
    }
}
