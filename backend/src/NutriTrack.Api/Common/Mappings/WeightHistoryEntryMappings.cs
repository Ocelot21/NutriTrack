using Mapster;
using NutriTrack.Application.WeightHistoryEntries.Common;
using NutriTrack.Contracts.WeightHistoryEntries;

namespace NutriTrack.Api.Common.Mappings;

public class WeightHistoryEntryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<WeightHistoryEntryResult, WeightHistoryEntryResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.Date, src => src.Date)
            .Map(dest => dest.WeightKg, src => src.WeightKg);

        config.NewConfig<IReadOnlyList<WeightHistoryEntryResult>, IReadOnlyList<WeightHistoryEntryResponse>>()
            .Map(dest => dest, src => src.Select(
                e => new WeightHistoryEntryResponse(
                    e.Id.Value,
                    e.UserId.Value,
                    e.Date,
                    e.WeightKg)));
    }
}
