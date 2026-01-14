using NutriTrack.Application.Common.Interfaces.Persistence;
using NutriTrack.Domain.Countries;

namespace NutriTrack.Infrastructure.Persistence.Repositories
{
    public class CountryRepository : EfRepository<Country, CountryCode>, ICountryRepository
    {
        public CountryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
