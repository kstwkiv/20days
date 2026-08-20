// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Notification.Infrastructure;
using NoCap.Eats.Notification.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console());

    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Auto-migrate on startup
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        await db.Database.MigrateAsync();
    }

    // Health check endpoint — lets load balancers / orchestrators probe this worker
    app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "notification" }));

    Log.Information("Notification service ready → http://localhost:5204/health");
    Log.Information("Notification service started — listening for events via RabbitMQ");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Notification service terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
