using ErrorOr;
using MediatR;
using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Application.Metadata.Common;

namespace NutriTrack.Application.Metadata.Queries.ListCountries;

public class ListCountriesQueryHandler : IRequestHandler<ListCountriesQuery, ErrorOr<IReadOnlyList<CountryResult>>>
{
    private readonly ICountryRepository _countryRepository;

    public ListCountriesQueryHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<CountryResult>>> Handle(
        ListCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countries = await _countryRepository.ListAsync(cancellationToken);
        return countries.Select(c => new CountryResult(c.Id, c.Name)).ToList();
    }
}
