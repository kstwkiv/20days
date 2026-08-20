// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Infrastructure.Messaging.Consumers;
using NoCap.Eats.Delivery.Infrastructure.Persistence;
using NoCap.Eats.Delivery.Infrastructure.Persistence.Repositories;

namespace NoCap.Eats.Delivery.Infrastructure;

/// <summary>Extension methods for registering Delivery Infrastructure services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core, the delivery repository, and MassTransit/RabbitMQ
    /// (including the <see cref="OrderPlacedConsumer"/>) for the Delivery service.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings and broker settings.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<DeliveryDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("DeliveryDb"),
                sql => sql.MigrationsAssembly(typeof(DeliveryDbContext).Assembly.FullName)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
        services.AddMassTransit(x =>
        {
            // Register the consumer that creates Delivery records when orders are placed
            x.AddConsumer<OrderPlacedConsumer>();

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
