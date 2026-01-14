namespace NutriTrack.Contracts.Metadata;

public record CountryListResponse(
    IReadOnlyList<CountryResponse> Countries);
