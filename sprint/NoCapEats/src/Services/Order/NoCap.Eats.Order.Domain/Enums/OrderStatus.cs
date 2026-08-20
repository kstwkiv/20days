// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Order.Domain.Enums;

/// <summary>
/// Represents the lifecycle states an order passes through from placement to completion.
/// The allowed transitions are enforced by the <see cref="Entities.Order"/> aggregate.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order just placed, awaiting restaurant confirmation.</summary>
    Pending        = 0,

    /// <summary>Restaurant accepted the order.</summary>
    Confirmed      = 1,

    /// <summary>Kitchen is actively preparing the food.</summary>
    Preparing      = 2,

    /// <summary>Food is ready and waiting for a delivery agent to pick it up.</summary>
    ReadyForPickup = 3,

    /// <summary>A delivery agent has picked up the order and is en route.</summary>
    OutForDelivery = 4,

    /// <summary>Order successfully handed to the customer.</summary>
    Delivered      = 5,

    /// <summary>Order was cancelled by the customer or restaurant.</summary>
    Cancelled      = 6,

    /// <summary>Restaurant declined the order.</summary>
    Rejected       = 7
}
