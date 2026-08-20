// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NoCap.Eats.Delivery.API.Endpoints;
using NoCap.Eats.Delivery.API.Middleware;
using NoCap.Eats.Delivery.Application;
using NoCap.Eats.Delivery.Infrastructure;
using NoCap.Eats.Delivery.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

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
    var jwt = builder.Configuration.GetSection("Jwt");
    var jwtSecret   = jwt["Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
    var jwtIssuer   = jwt["Issuer"]
        ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
    var jwtAudience = jwt["Audience"]
        ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

    if (jwtSecret.StartsWith("CHANGE_ME"))
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
                ValidIssuer              = jwtIssuer,
                ValidAudience            = jwtAudience,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwtSecret)),
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
        opts.AddPolicy("DeliveryAgent", p =>
            p.RequireAuthenticatedUser().RequireRole("DeliveryAgent")));

    // ── Swagger ───────────────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "NoCap Eats: Delivery API", Version = "v1",
            Description = "Delivery agents accept and track food deliveries." });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization", Type = SecuritySchemeType.Http,
            Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
            Description = "Paste your JWT token. Obtain one from the Identity API POST /api/auth/login."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                    { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<DeliveryDbContext>().Database.MigrateAsync();
    }

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Delivery API v1"));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapDeliveryEndpoints();

    Log.Information("Delivery API ready → http://localhost:5203/swagger");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Delivery service terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
