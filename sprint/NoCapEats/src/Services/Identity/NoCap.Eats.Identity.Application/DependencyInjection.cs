// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Identity.Application.Behaviours;

namespace NoCap.Eats.Identity.Application;

/// <summary>Extension methods for registering Identity Application layer services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers MediatR handlers, FluentValidation validators,
    /// the validation pipeline behaviour, and AutoMapper profiles
    /// from the Identity Application assembly.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Register all IRequestHandler<,> and INotificationHandler<> implementations
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Register all AbstractValidator<T> implementations for FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // Insert the validation pipeline — runs before every MediatR handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        // Register AutoMapper profiles for DTO mapping
        services.AddAutoMapper(assembly);

        return services;
    }
}
