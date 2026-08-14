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

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseSqlServer(
                configuration.GetConnectionString("IdentityDb"),
                sql => sql.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        // ── JWT Settings ──────────────────────────────────────────────────────
        services.Configure<JwtSettings>(opts =>
            configuration.GetSection(JwtSettings.SectionName).Bind(opts));

        // ── Repositories & Services ───────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IdentityDbSeeder>();

        // ── MassTransit / RabbitMQ ────────────────────────────────────────────
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
