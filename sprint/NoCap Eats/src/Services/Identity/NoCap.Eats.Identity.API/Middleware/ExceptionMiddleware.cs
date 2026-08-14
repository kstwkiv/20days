using System.Net;
using System.Text.Json;
using FluentValidation;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
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
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            EmailAlreadyRegisteredException => (HttpStatusCode.Conflict, ex.Message, (Dictionary<string, string[]>?)null),
            InvalidCredentialsException     => (HttpStatusCode.Unauthorized, ex.Message, null),
            AccountDeactivatedException     => (HttpStatusCode.Forbidden, ex.Message, null),
            InvalidRefreshTokenException    => (HttpStatusCode.Unauthorized, ex.Message, null),
            UserNotFoundException           => (HttpStatusCode.NotFound, ex.Message, null),
            DomainException                 => (HttpStatusCode.BadRequest, ex.Message, null),
            _                               => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)status;

        var body = errors is not null
            ? new { title, errors }
            : (object)new { title };

        return context.Response.WriteAsync(JsonSerializer.Serialize(body, _jsonOpts));
    }
}
