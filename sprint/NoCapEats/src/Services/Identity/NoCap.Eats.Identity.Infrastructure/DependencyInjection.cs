// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Infrastructure.Persistence;
using NoCap.Eats.Identity.Infrastructure.Persistence.Repositories;
using NoCap.Eats.Identity.Infrastructure.Services;
using NoCap.Eats.Identity.Infrastructure.Settings;

namespace NoCap.Eats.Identity.Infrastructure;

/// <summary>
/// Extension methods for registering Identity Infrastructure services with the DI container.
/// Covers EF Core, JWT settings, repositories, token service, role seeder, and MassTransit.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure-layer services required by the Identity service.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings and settings.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        // Register EF Core with SQL Server; migrations assembly is this project
        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("IdentityDb"),
                sql => sql.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        // ── JWT Settings ──────────────────────────────────────────────────────
        // Bind the "Jwt" config section to JwtSettings for use in TokenService
        services.Configure<JwtSettings>(opts =>
            configuration.GetSection(JwtSettings.SectionName).Bind(opts));

        // ── Repositories & Services ───────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenService, TokenService>();

        // Seeder is registered as scoped and called manually from Program.cs on startup
        services.AddScoped<IdentityDbSeeder>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
        // Configure MassTransit to publish integration events to RabbitMQ
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"] ?? "rabbitmq://localhost", h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "guest");
                    h.Password(configuration["RabbitMq:Password"] ?? "guest");
                });

                // Auto-configure send/receive endpoints from registered consumers
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
