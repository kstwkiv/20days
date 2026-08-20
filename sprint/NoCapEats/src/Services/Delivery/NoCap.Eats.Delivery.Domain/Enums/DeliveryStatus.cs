// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Delivery.Domain.Enums;

/// <summary>
/// Lifecycle states of a delivery job, progressing from creation to completion.
/// Transitions are enforced by the <see cref="Entities.Delivery"/> aggregate.
/// </summary>
public enum DeliveryStatus
{
    /// <summary>Delivery created but no agent has accepted the job yet.</summary>
    Pending   = 0,

    /// <summary>An agent has self-assigned and accepted the job.</summary>
    Assigned  = 1,

    /// <summary>Agent has physically collected the food from the restaurant.</summary>
    PickedUp  = 2,

    /// <summary>Food successfully handed to the customer.</summary>
    Delivered = 3,

    /// <summary>Delivery could not be completed (customer not available, wrong address, etc.).</summary>
    Failed    = 4
}
