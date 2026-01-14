using ErrorOr;
using MediatR;
using NutriTrack.Application.Metadata.Common;

namespace NutriTrack.Application.Metadata.Queries.ListCountries;

public record ListCountriesQuery() : IRequest<ErrorOr<IReadOnlyList<CountryResult>>>;