using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Identity.Application.Behaviours;

namespace NoCap.Eats.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddAutoMapper(assembly);

        return services;
    }
}
