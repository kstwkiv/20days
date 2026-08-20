// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Net;
using System.Text.Json;
using FluentValidation;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.API.Middleware;

/// <summary>
/// Global exception handling middleware for the Identity API.
/// Catches all unhandled exceptions, maps them to appropriate HTTP status codes,
/// and writes a consistent camelCase JSON error body.
/// Prevents raw exception details from leaking to clients.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    /// <summary>Shared JSON options that apply camelCase property naming to all error responses.</summary>
    private static readonly JsonSerializerOptions _jsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>
    /// Invokes the next middleware and catches any unhandled exception.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the full exception before converting to a client-safe response
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    /// <summary>
    /// Maps a caught exception to an HTTP status code and writes the JSON error response.
    /// </summary>
    private static Task HandleAsync(HttpContext context, Exception ex)
    {
        // Pattern-match to determine the correct status code and response shape
        var (status, title, errors) = ex switch
        {
            // FluentValidation failures → 400 with field-level error details
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            EmailAlreadyRegisteredException => (HttpStatusCode.Conflict,       ex.Message, (Dictionary<string, string[]>?)null),
            InvalidCredentialsException     => (HttpStatusCode.Unauthorized,   ex.Message, null),
            AccountDeactivatedException     => (HttpStatusCode.Forbidden,      ex.Message, null),
            InvalidRefreshTokenException    => (HttpStatusCode.Unauthorized,   ex.Message, null),
            UserNotFoundException           => (HttpStatusCode.NotFound,       ex.Message, null),
            DomainException                 => (HttpStatusCode.BadRequest,     ex.Message, null),
            // Unrecognised exceptions become 500 without exposing internal details
            _                               => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        // Include field-level errors for validation failures; plain title otherwise
        var body = errors is not null
            ? new { title, errors }
            : (object)new { title };

        return context.Response.WriteAsync(JsonSerializer.Serialize(body, _jsonOpts));
    }
}
