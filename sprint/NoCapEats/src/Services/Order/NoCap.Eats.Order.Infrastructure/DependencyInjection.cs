// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Order.Application.Interfaces;
using NoCap.Eats.Order.Infrastructure.Persistence;
using NoCap.Eats.Order.Infrastructure.Persistence.Repositories;

namespace NoCap.Eats.Order.Infrastructure;

/// <summary>Extension methods for registering Order Infrastructure services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core, the order repository, and MassTransit/RabbitMQ
    /// for the Order service.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings and broker settings.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<OrderDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("OrderDb"),
                sql => sql.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IOrderRepository, OrderRepository>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
        // Order service is a publisher only; no consumers are registered here
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"] ?? "rabbitmq://localhost", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
