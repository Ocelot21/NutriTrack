using NutriTrack.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
