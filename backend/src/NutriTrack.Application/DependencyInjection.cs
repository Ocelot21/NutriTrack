using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NutriTrack.Application.Common.Behaviors;
using System.Reflection;

namespace NutriTrack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(DomainExceptionBehavior<,>));

        return services;
    }
}