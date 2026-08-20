// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoCap.Eats.Identity.Application.Commands.Login;
using NoCap.Eats.Identity.Application.Commands.RefreshToken;
using NoCap.Eats.Identity.Application.Commands.Register;
using NoCap.Eats.Identity.Application.Commands.RevokeToken;
using NoCap.Eats.Identity.Application.Queries.GetCurrentUser;
using System.Security.Claims;

namespace NoCap.Eats.Identity.API.Endpoints;

/// <summary>
/// Minimal API endpoint definitions for authentication and user profile operations.
/// All routes are grouped under /api/auth and tagged "Auth" for Swagger grouping.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Registers all authentication endpoints on the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to map routes onto.</param>
    /// <returns>The same <paramref name="app"/> instance for chaining.</returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/register — open to anonymous callers
        group.MapPost("/register", async (
            [FromBody] RegisterCommand command,
            ISender sender) =>
        {
            var user = await sender.Send(command);
            // Return 201 Created with a Location header pointing to the new resource
            return Results.Created($"/api/users/{user.Id}", user);
        })
        .WithName("Register")
        .WithSummary("Register a new user account.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

        // POST /api/auth/login — returns JWT + refresh token on success
        group.MapPost("/login", async (
            [FromBody] LoginCommand command,
            ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithSummary("Authenticate and receive JWT + refresh token.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/auth/refresh — rotates the refresh token and issues a new access token
        group.MapPost("/refresh", async (
            [FromBody] RefreshTokenCommand command,
            ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithSummary("Rotate a refresh token and get a new access token.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/auth/revoke — requires a valid JWT; revokes all refresh tokens (logout)
        group.MapPost("/revoke", async (
            HttpContext http,
            ISender sender) =>
        {
            // Extract the user ID from the "sub" or NameIdentifier claim
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? http.User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var uid))
                return Results.Unauthorized();

            await sender.Send(new RevokeTokenCommand(uid));
            return Results.NoContent();
        })
        .WithName("RevokeToken")
        .WithSummary("Logout, revoke all refresh tokens.")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized);

        // GET /api/auth/me — returns the authenticated user's full profile
        group.MapGet("/me", async (
            HttpContext http,
            ISender sender,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("AuthEndpoints.Me");

            // Extract the user ID from the JWT claim
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? http.User.FindFirstValue("sub");

            logger.LogInformation("GET /me — claims: {Claims}",
                string.Join(", ", http.User.Claims.Select(c => $"{c.Type}={c.Value}")));

            logger.LogInformation("GET /me — extracted userId: {UserId}", userId);

            if (!Guid.TryParse(userId, out var uid))
            {
                logger.LogWarning("GET /me — could not parse userId as Guid: {Raw}", userId);
                return Results.Unauthorized();
            }

            var user = await sender.Send(new GetCurrentUserQuery(uid)); 
            return Results.Ok(user);
        })
        .WithName("GetCurrentUser")
        .WithSummary("Get the authenticated user's profile.")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
