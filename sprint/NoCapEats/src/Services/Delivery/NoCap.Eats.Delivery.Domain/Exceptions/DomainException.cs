// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Delivery.Domain.Exceptions;

/// <summary>Base exception for all domain-level errors in the Delivery service.</summary>
/// <param name="message">Human-readable description of the error.</param>
public class DomainException(string message) : Exception(message);

/// <summary>Thrown when a delivery record with the given ID cannot be found.</summary>
/// <param name="id">The delivery ID that was searched for.</param>
public class DeliveryNotFoundException(Guid id)
    : DomainException($"Delivery '{id}' was not found.");

/// <summary>Thrown when an agent attempts to accept a delivery that is already assigned.</summary>
/// <param name="orderId">The order ID of the already-assigned delivery.</param>
public class DeliveryAlreadyAssignedException(Guid orderId)
    : DomainException($"Order '{orderId}' already has a delivery agent assigned.");

/// <summary>Thrown when an invalid delivery status transition is attempted.</summary>
/// <param name="from">The current status string.</param>
/// <param name="to">The target status that cannot be reached from <paramref name="from"/>.</param>
public class InvalidDeliveryStatusTransitionException(string from, string to)
    : DomainException($"Cannot transition delivery from '{from}' to '{to}'.");

/// <summary>Thrown when an agent attempts to act on a delivery they did not accept.</summary>
/// <param name="agentId">The ID of the agent making the request.</param>
/// <param name="deliveryId">The ID of the delivery being accessed.</param>
public class UnauthorizedDeliveryAccessException(Guid agentId, Guid deliveryId)
    : DomainException($"Agent '{agentId}' does not own delivery '{deliveryId}'.");
