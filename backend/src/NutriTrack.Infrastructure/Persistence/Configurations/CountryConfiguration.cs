using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriTrack.Domain.Common;
using NutriTrack.Domain.Countries;

namespace NutriTrack.Infrastructure.Persistence.Configurations
{
    internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.
                Property(c => c.Id)
                .HasCountryCodeConversion()
                .HasMaxLength(2);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(DomainConstraints.Countries.MaxCountryNameLength);


            builder.ConfigureAuditable();
        }
    }
}