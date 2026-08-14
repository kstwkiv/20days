using MediatR;
using Microsoft.AspNetCore.Mvc;
using NoCap.Eats.Identity.Application.Commands.Login;
using NoCap.Eats.Identity.Application.Commands.RefreshToken;
using NoCap.Eats.Identity.Application.Commands.Register;
using NoCap.Eats.Identity.Application.Commands.RevokeToken;
using NoCap.Eats.Identity.Application.Queries.GetCurrentUser;
using System.Security.Claims;

namespace NoCap.Eats.Identity.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            [FromBody] RegisterCommand command,
            ISender sender) =>
        {
            var user = await sender.Send(command);
            return Results.Created($"/api/users/{user.Id}", user);
        })
        .WithName("Register")
        .WithSummary("Register a new user account.")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status409Conflict);

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

        group.MapPost("/revoke", async (
            HttpContext http,
            ISender sender) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? http.User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var uid))
                return Results.Unauthorized();

            await sender.Send(new RevokeTokenCommand(uid));
            return Results.NoContent();
        })
        .WithName("RevokeToken")
        .WithSummary("Logout — revoke all refresh tokens for the current user.")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", async (
            HttpContext http,
            ISender sender) =>
        {
            var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? http.User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var uid))
                return Results.Unauthorized();

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
