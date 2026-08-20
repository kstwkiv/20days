// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoCap.Eats.Order.API.Endpoints;
using NoCap.Eats.Order.API.Middleware;
using NoCap.Eats.Order.Application;
using NoCap.Eats.Order.Infrastructure;
using NoCap.Eats.Order.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console());

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── JWT ───────────────────────────────────────────────────────────────────
    var jwt      = builder.Configuration.GetSection("Jwt");
    var secret   = jwt["Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
    var issuer   = jwt["Issuer"]
        ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
    var audience = jwt["Audience"]
        ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

    if (secret.StartsWith("CHANGE_ME"))
        throw new InvalidOperationException("Jwt:Secret is still the placeholder. Set a real secret.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = issuer,
                ValidAudience            = audience,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(secret)),
                ClockSkew                = TimeSpan.Zero
            };
            opts.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Log.Warning("JWT authentication failed: {Error}", ctx.Exception.Message);
                    return Task.CompletedTask;
                },
                OnChallenge = ctx =>
                {
                    if (ctx.AuthenticateFailure is not null)
                        Log.Warning("JWT challenge triggered: {Reason}", ctx.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization(opts =>
    {
        opts.AddPolicy("RestaurantOwner", p =>
            p.RequireAuthenticatedUser().RequireRole("RestaurantOwner"));
    });

    // ── Swagger ───────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "NoCap Eats — Order API",
            Version     = "v1",
            Description = "Place orders, track status, and manage restaurant incoming orders."
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "Bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "Paste your JWT token. Obtain one from the Identity API POST /api/auth/login."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                        { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API v1"));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapOrderEndpoints();

    Log.Information("Order API ready → http://localhost:5202/swagger");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Order service terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
