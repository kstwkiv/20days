// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.Net;
using System.Text.Json;
using FluentValidation;
using NoCap.Eats.Delivery.Domain.Exceptions;

namespace NoCap.Eats.Delivery.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _opts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleAsync(context, ex);
        }
    }

    private static Task HandleAsync(HttpContext context, Exception ex)
    {
        var (status, title, errors) = ex switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest, "Validation failed.",
                ve.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            UnauthorizedDeliveryAccessException    => (HttpStatusCode.Forbidden,  ex.Message, (Dictionary<string,string[]>?)null),
            DeliveryNotFoundException              => (HttpStatusCode.NotFound,   ex.Message, null),
            DeliveryAlreadyAssignedException       => (HttpStatusCode.Conflict,   ex.Message, null),
            InvalidDeliveryStatusTransitionException => (HttpStatusCode.Conflict, ex.Message, null),
            DomainException                        => (HttpStatusCode.BadRequest, ex.Message, null),
            _                                      => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;
        var body = errors is not null ? new { title, errors } : (object)new { title };
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, _opts));
    }
}
