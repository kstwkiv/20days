// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.BuildingBlocks.Events;

/// <summary>Published whenever an order changes status.</summary>
/// <param name="OrderId">Unique identifier of the order whose status changed.</param>
/// <param name="CustomerId">Identifier of the customer who placed the order.</param>
/// <param name="RestaurantId">Identifier of the restaurant fulfilling the order.</param>
/// <param name="OldStatus">Status value before the transition.</param>
/// <param name="NewStatus">Status value after the transition.</param>
/// <param name="ChangedAt">UTC timestamp when the status change occurred.</param>
public record OrderStatusChangedEvent(
    Guid   OrderId,
    Guid   CustomerId,
    Guid   RestaurantId,
    string OldStatus,
    string NewStatus,
    DateTime ChangedAt);
