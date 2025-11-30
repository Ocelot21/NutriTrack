using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NutriTrack.Application.Common.Interfaces.Authentication;
using NutriTrack.Application.Common.Interfaces.Authorization;
using NutriTrack.Application.Common.Interfaces.Services;
using NutriTrack.Domain.Users;
using NutriTrack.Infrastructure.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NutriTrack.Infrastructure.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPermissionProvider _permissionProvider;

        public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions, IPermissionProvider permissionProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtOptions.Value;
            _permissionProvider = permissionProvider;
        }


        public async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.ToString()),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.UniqueName, user.Username.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roleName = await _permissionProvider.GetRoleNameAsync(user.Id, cancellationToken);
            if (!string.IsNullOrWhiteSpace(roleName))
                claims.Add(new Claim(AuthClaimTypes.Role, roleName!));

            var perms = await _permissionProvider.GetForUserAsync(user.Id, cancellationToken);
            foreach (var p in perms)
                claims.Add(new Claim(AuthClaimTypes.Perms, p));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                claims: claims,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}