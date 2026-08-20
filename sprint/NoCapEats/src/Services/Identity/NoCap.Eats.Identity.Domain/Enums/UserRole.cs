// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Domain.Enums;

/// <summary>Defines the role name constants used throughout the Identity service.</summary>
public static class UserRole
{
    /// <summary>Standard customer who places food orders.</summary>
    public const string Customer          = "Customer";

    /// <summary>Business owner who manages one or more restaurants.</summary>
    public const string RestaurantOwner   = "RestaurantOwner";

    /// <summary>Agent responsible for delivering orders to customers.</summary>
    public const string DeliveryAgent     = "DeliveryAgent";

    /// <summary>Platform administrator with elevated privileges.</summary>
    public const string Admin             = "Admin";

    /// <summary>Read-only collection of all defined role names, used for seeding and validation.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        Customer, RestaurantOwner, DeliveryAgent, Admin
    ];
}
