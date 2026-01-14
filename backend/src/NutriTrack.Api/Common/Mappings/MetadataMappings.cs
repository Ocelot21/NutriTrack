using Mapster;
using NutriTrack.Application.Metadata.Common;
using NutriTrack.Contracts.Metadata;

namespace NutriTrack.Api.Common.Mappings
{
    public class MetadataMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CountryResult, CountryResponse>()
                .Map(dest => dest.Code, src => src.CountryCode.Value)
                .Map(dest => dest.Name, src => src.Name);

            config.NewConfig<IReadOnlyList<CountryResult>, CountryListResponse>()
                .Map(dest => dest.Countries, src 
                => src.Select(x => new CountryResponse(x.CountryCode.Value, x.Name)).ToList());
        }
    }
}
