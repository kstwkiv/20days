// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoCap.Eats.Catalog.API.Endpoints;
using NoCap.Eats.Catalog.API.Middleware;
using NoCap.Eats.Catalog.Application;
using NoCap.Eats.Catalog.Infrastructure;
using NoCap.Eats.Catalog.Infrastructure.Persistence;
using Serilog;

// ── Bootstrap logger ──────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Serilog ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console());

    // ── Application + Infrastructure ──────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── JWT Authentication (validate tokens issued by Identity service) ───────
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var secret     = jwtSection["Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
    var issuer     = jwtSection["Issuer"]
        ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
    var audience   = jwtSection["Audience"]
        ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

    if (secret.StartsWith("CHANGE_ME"))
        throw new InvalidOperationException(
            "Jwt:Secret is still the placeholder value. Set a real secret before running.");

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
            p.RequireAuthenticatedUser()
             .RequireRole("RestaurantOwner"));
    });

    // ── Swagger / OpenAPI ─────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "NoCap Eats: Catalog API",
            Version     = "v1",
            Description = "Browse restaurants and menus. Owners manage their listings."
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

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Auto-migrate on startup ───────────────────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.Database.MigrateAsync();
    }

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1"));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // ── Endpoints ─────────────────────────────────────────────────────────────
    app.MapCatalogEndpoints();

    Log.Information("Catalog API ready → http://localhost:5201/swagger");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Catalog service terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
