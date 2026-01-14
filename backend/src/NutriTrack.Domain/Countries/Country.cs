using NutriTrack.Domain.Common.Models;

namespace NutriTrack.Domain.Countries
{
    public class Country : AggregateRoot<CountryCode>
    {
        [Obsolete("Constructor for EF Core only", error: false)]
        private Country() : base()
        {

        }

        private Country(CountryCode id, string name) : base(id)
        {
            Name = name;
        }

        public string Name { get; private set; } = null!;

        public static Country Create(CountryCode id, string name)
        {
            return new Country(id, name);
        }
    }
}
