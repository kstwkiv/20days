// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoCap.Eats.Identity.API.Endpoints;
using NoCap.Eats.Identity.API.Middleware;
using NoCap.Eats.Identity.Application;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Infrastructure;
using NoCap.Eats.Identity.Infrastructure.Persistence;
using NoCap.Eats.Identity.Infrastructure.Settings;
using Serilog;

// ── Bootstrap logger ──────────────────────────────────────────────────────────
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

    // ── Application + Infrastructure ──────────────────────────────────────────
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── ASP.NET Core Identity ─────────────────────────────────────────────────
    builder.Services
        .AddIdentity<AppUser, IdentityRole<Guid>>(opts =>
        {
            opts.Password.RequireDigit           = true;
            opts.Password.RequiredLength         = 8;
            opts.Password.RequireUppercase       = true;
            opts.Password.RequireNonAlphanumeric = false;
            opts.User.RequireUniqueEmail         = true;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

    // ── JWT Authentication ────────────────────────────────────────────────────
    var jwtSettings = builder.Configuration
        .GetSection(JwtSettings.SectionName)
        .Get<JwtSettings>()!;

    // Guard: fail fast if the JWT secret is missing or still the placeholder value
    if (string.IsNullOrWhiteSpace(jwtSettings.Secret) ||
        jwtSettings.Secret.StartsWith("CHANGE_ME"))
        throw new InvalidOperationException(
            "JWT Secret is not configured. Set Jwt:Secret in appsettings or environment variables.");

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
                ValidIssuer              = jwtSettings.Issuer,
                ValidAudience            = jwtSettings.Audience,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew                = TimeSpan.Zero
            };
            // Log JWT auth failures so they appear in Serilog output
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

    builder.Services.AddAuthorization();

    // ── Swagger / OpenAPI ─────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "NoCap Eats: Identity API",
            Version     = "v1",
            Description = "Authentication, registration, token refresh and user profile endpoints."
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "Bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "Paste your JWT token here. Obtain one from POST /api/auth/login."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Auto-migrate + seed on startup (dev convenience) ─────────────────────
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<IdentityDbSeeder>();
        await seeder.SeedRolesAsync();
    }

    // ── Middleware pipeline ───────────────────────────────────────────────────
    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1"));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // ── Endpoints ─────────────────────────────────────────────────────────────
    app.MapAuthEndpoints();

    Log.Information("Identity API ready → http://localhost:5200/swagger");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Identity service terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
