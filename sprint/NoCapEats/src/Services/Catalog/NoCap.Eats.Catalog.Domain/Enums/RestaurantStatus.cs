// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Catalog.Domain.Enums;

/// <summary>
/// Represents the administrative and operational state of a restaurant listing.
/// New restaurants start in <see cref="PendingApproval"/> until reviewed by an Admin.
/// </summary>
public enum RestaurantStatus
{
    /// <summary>Newly created restaurant awaiting platform approval before going live.</summary>
    PendingApproval = 0,

    /// <summary>Approved and visible to customers for ordering.</summary>
    Active          = 1,

    /// <summary>Temporarily suspended by the platform; hidden from customers.</summary>
    Suspended       = 2,

    /// <summary>Permanently closed; removed from active listings.</summary>
    Closed          = 3
}
