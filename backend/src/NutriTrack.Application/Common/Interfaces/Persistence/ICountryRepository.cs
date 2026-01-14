using NutriTrack.Domain.Countries;

namespace NutriTrack.Application.Common.Interfaces.Persistence
{
    public interface ICountryRepository : IRepository<Country, CountryCode>
    {

    }
}
