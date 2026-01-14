using Microsoft.EntityFrameworkCore;
using NutriTrack.Domain.Countries;

namespace NutriTrack.Infrastructure.Persistence.Seed.Countries;

public sealed class CountrySeeder : ISeeder
{
    private readonly AppDbContext _dbContext;

    public CountrySeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int Order => 40;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var countries = new (string Iso2, string Name)[]
        {
            ("AU", "Australia"),
            ("BR", "Brazil"),
            ("CA", "Canada"),
            ("DE", "Germany"),
            ("ES", "Spain"),
            ("FR", "France"),
            ("GB", "United Kingdom"),
            ("IN", "India"),
            ("IT", "Italy"),
            ("JP", "Japan"),
            ("MX", "Mexico"),
            ("NL", "Netherlands"),
            ("NO", "Norway"),
            ("PL", "Poland"),
            ("SE", "Sweden"),
            ("TR", "Turkey"),
            ("UA", "Ukraine"),
            ("US", "United States"),
            ("AL", "Albania"),
            ("BA", "Bosnia and Herzegovina"),
            ("BG", "Bulgaria"),
            ("HR", "Croatia"),
            ("GR", "Greece"),
            ("ME", "Montenegro"),
            ("MK", "North Macedonia"),
            ("RO", "Romania"),
            ("RS", "Serbia"),
            ("SI", "Slovenia"),
            ("XK", "Kosovo"),
        };

        var existingIds = await _dbContext.Countries
            .Select(c => c.Id.Value)
            .ToListAsync(cancellationToken);

        var existingSet = new HashSet<string>(existingIds, StringComparer.OrdinalIgnoreCase);

        var toInsert = countries
            .Where(c => !existingSet.Contains(c.Iso2))
            .Select(c => Country.Create(CountryCode.Create(c.Iso2), c.Name))
            .ToArray();

        if (toInsert.Length > 0)
        {
            _dbContext.Countries.AddRange(toInsert);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await tx.CommitAsync(cancellationToken);
    }
}
