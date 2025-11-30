using Mapster;
using NutriTrack.Application.Authentication.Commands.Login;
using NutriTrack.Application.Authentication.Commands.Register;
using NutriTrack.Application.Authentication.Common;
using NutriTrack.Contracts.Authentication;

namespace NutriTrack.Api.Common.Mappings;

public class AuthenticationMappings : IRegister
{

    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();

        config.NewConfig<LoginRequest, LoginCommand>();

        config.NewConfig<AuthenticationResult, AuthenticationResponse>();
    }
}
