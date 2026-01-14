using Mapster;
using NutriTrack.Application.Roles.Common;
using NutriTrack.Contracts.Roles;

namespace NutriTrack.Api.Common.Mappings
{
    public class RolesMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RoleResult, RoleResponse>()
                .Map(dest => dest.Id, src => src.Id.Value)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Permissions, src => src.Permissions);
        }
    }
}
