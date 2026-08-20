// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Notification.Application.Interfaces;
using NoCap.Eats.Notification.Infrastructure.Email;
using NoCap.Eats.Notification.Infrastructure.Messaging.Consumers;
using NoCap.Eats.Notification.Infrastructure.Persistence;
using NoCap.Eats.Notification.Infrastructure.Persistence.Repositories;

namespace NoCap.Eats.Notification.Infrastructure;

/// <summary>Extension methods for registering Notification Infrastructure services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core, the notification log repository, the email sender,
    /// and MassTransit/RabbitMQ with all three event consumers.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings and broker settings.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<NotificationDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("NotificationDb"),
                sql => sql.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName)));

        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

        // ── Email Sender ──────────────────────────────────────────────────────
        // Development: logs to console. Production: swap to SendGridEmailSender
        services.AddScoped<IEmailSender, ConsoleEmailSender>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
        services.AddMassTransit(x =>
        {
            // Register all three event consumers
            x.AddConsumer<UserRegisteredConsumer>();
            x.AddConsumer<OrderPlacedConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();

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
