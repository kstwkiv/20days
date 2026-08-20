// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Infrastructure.Messaging.Consumers;
using NoCap.Eats.Catalog.Infrastructure.Persistence;
using NoCap.Eats.Catalog.Infrastructure.Persistence.Repositories;
using NoCap.Eats.Catalog.Infrastructure.Services;
using NoCap.Eats.Catalog.Infrastructure.Settings;

namespace NoCap.Eats.Catalog.Infrastructure;

/// <summary>Extension methods for registering Catalog Infrastructure services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core, all three repositories, Azure Blob Storage,
    /// and MassTransit/RabbitMQ (with the <see cref="UserRegisteredConsumer"/>) for the Catalog service.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for connection strings and settings.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("CatalogDb"),
                sql => sql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName)));

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();

        // ── Azure Blob Storage ────────────────────────────────────────────────
        services.Configure<StorageSettings>(opts =>
            configuration.GetSection(StorageSettings.SectionName).Bind(opts));

        // Register the Azure SDK client; falls back to Azurite in development
        services.AddSingleton(_ =>
            new BlobServiceClient(
                configuration[$"{StorageSettings.SectionName}:ConnectionString"]
                ?? "UseDevelopmentStorage=true"));

        services.AddScoped<IImageStorageService, AzureBlobImageStorageService>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserRegisteredConsumer>();

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
