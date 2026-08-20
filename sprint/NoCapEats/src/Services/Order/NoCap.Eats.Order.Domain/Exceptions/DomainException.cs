// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Order.Domain.Exceptions;

/// <summary>Base exception for all domain-level errors in the Order service.</summary>
/// <param name="message">Human-readable description of the error.</param>
public class DomainException(string message) : Exception(message);

/// <summary>Thrown when an order with the given ID cannot be found.</summary>
/// <param name="id">The order ID that was searched for.</param>
public class OrderNotFoundException(Guid id)
    : DomainException($"Order '{id}' was not found.");

/// <summary>Thrown when attempting an invalid status transition on an order.</summary>
/// <param name="from">The current status string.</param>
/// <param name="to">The target status string that is not reachable from <paramref name="from"/>.</param>
public class InvalidOrderStatusTransitionException(string from, string to)
    : DomainException($"Cannot transition order from '{from}' to '{to}'.");

/// <summary>Thrown when a customer attempts to cancel an order that is past a cancellable state.</summary>
/// <param name="id">The ID of the order that cannot be cancelled.</param>
public class OrderNotCancellableException(Guid id)
    : DomainException($"Order '{id}' cannot be cancelled in its current state.");

/// <summary>Thrown when a user attempts to access or modify an order they do not own.</summary>
/// <param name="userId">The ID of the user making the request.</param>
/// <param name="orderId">The ID of the order being accessed.</param>
public class UnauthorizedOrderAccessException(Guid userId, Guid orderId)
    : DomainException($"User '{userId}' does not have access to order '{orderId}'.");

/// <summary>Thrown when an order is created with no line items.</summary>
public class EmptyOrderException()
    : DomainException("An order must contain at least one item.");
