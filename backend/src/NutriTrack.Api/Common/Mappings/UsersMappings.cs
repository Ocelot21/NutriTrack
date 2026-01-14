using Mapster;
using NutriTrack.Application.Common.Models;
using NutriTrack.Contracts.Common;
using NutriTrack.Contracts.Users;

namespace NutriTrack.Api.Common.Mappings;

public sealed class UsersMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserResult, UserResponse>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Username, src => src.Username.Value)
            .Map(dest => dest.Email, src => src.Email.Value)
            .Map(dest => dest.RoleId, src => src.Role == null ? (Guid?)null : src.Role.Id.Value)
            .Map(dest => dest.RoleName, src => src.Role == null ? null : src.Role.Name)
            .Map(dest => dest.CountryCode, src => src.Country == null ? null : src.Country.Value)
            .Map(dest => dest.Gender, src => src.Gender.ToString())
            .Map(dest => dest.ActivityLevel, src => src.ActivityLevel.ToString());

        config.NewConfig<PagedResult<UserResult>, PagedResponse<UserResponse>>()
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.TotalCount, src => src.TotalCount)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);
    }
}
