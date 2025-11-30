using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Domain.Authorization;

namespace NutriTrack.Infrastructure.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionPolicies(this IServiceCollection services, string permClaimType = "perms")
        {
            services.AddAuthorizationCore(options =>
            {
                foreach (var key in PermissionKeys.All)
                {
                    options.AddPolicy(key, p => p.RequireClaim(permClaimType, key));
                }
            });

            return services;
        }
    }
}