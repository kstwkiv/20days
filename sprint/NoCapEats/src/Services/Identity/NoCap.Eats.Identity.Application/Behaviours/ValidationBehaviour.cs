// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;
using MediatR;

namespace NoCap.Eats.Identity.Application.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that runs all registered FluentValidation validators
/// for a request before the handler executes.
/// Throws <see cref="ValidationException"/> if any validation rules fail,
/// preventing the handler from being called with invalid input.
/// </summary>
/// <typeparam name="TRequest">The MediatR request type being validated.</typeparam>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
/// <param name="validators">All validators registered for <typeparamref name="TRequest"/>.</param>
public class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Validates the request, then delegates to the next behaviour or handler.
    /// </summary>
    /// <param name="request">The incoming request to validate.</param>
    /// <param name="next">Delegate invoking the next step in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The handler's response if validation passes.</returns>
    /// <exception cref="ValidationException">Thrown when one or more validation rules fail.</exception>
    public async Task<TResponse> Handle(
        TRequest                          request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 cancellationToken)
    {
        // Skip validation if no validators are registered for this request type
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        // Collect all failure messages from every registered validator
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
