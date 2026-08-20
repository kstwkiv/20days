// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ── Configuration — load ocelot.json on top of appsettings ───────────────
    // In Docker, service names resolve via DNS; use ocelot.Docker.json when containerised.
    var ocelotFile = builder.Environment.IsEnvironment("Docker") || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"))
        ? "ocelot.Docker.json"
        : "ocelot.json";

    builder.Configuration.AddJsonFile(ocelotFile, optional: false, reloadOnChange: true);

    // ── Serilog ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .WriteTo.Console());

    // ── JWT Authentication (validates tokens before forwarding) ───────────────
    var jwt = builder.Configuration.GetSection("Jwt");
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = jwt["Issuer"]!,
                ValidAudience            = jwt["Audience"]!,
                IssuerSigningKey         = new SymmetricSecurityKey(
                                               Encoding.UTF8.GetBytes(jwt["Secret"]!)),
                ClockSkew                = TimeSpan.Zero
            };
        });

    // ── Ocelot + SwaggerForOcelot ─────────────────────────────────────────────
    builder.Services.AddOcelot();
    builder.Services.AddSwaggerForOcelot(builder.Configuration);

    // ── CORS — allow any origin for dev ──────────────────────────────────────
    builder.Services.AddCors(opts =>
        opts.AddDefaultPolicy(p =>
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    var app = builder.Build();

    app.UseCors();
    app.UseSerilogRequestLogging();

    // ── Swagger UI aggregating all downstream services ────────────────────────
    app.UseSwaggerForOcelotUI(opts =>
    {
        opts.PathToSwaggerGenerator = "/swagger/docs";
        opts.ReConfigureUpstreamSwaggerJson = (httpContext, swaggerJson) => swaggerJson;
    });

    app.UseAuthentication();

    // ── Ocelot middleware — must be last ──────────────────────────────────────
    await app.UseOcelot();

    Log.Information("API Gateway ready → http://localhost:5000/swagger");

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
