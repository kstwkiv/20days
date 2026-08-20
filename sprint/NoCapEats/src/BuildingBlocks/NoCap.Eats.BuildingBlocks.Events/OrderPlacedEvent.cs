// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.BuildingBlocks.Events;

/// <summary>Published when a customer places a new order.</summary>
/// <param name="OrderId">Unique identifier of the placed order.</param>
/// <param name="CustomerId">Identifier of the customer who placed the order.</param>
/// <param name="RestaurantId">Identifier of the restaurant fulfilling the order.</param>
/// <param name="CustomerName">Full name of the customer.</param>
/// <param name="CustomerPhone">Contact phone number of the customer.</param>
/// <param name="DeliveryAddress">Street address where the order should be delivered.</param>
/// <param name="TotalAmount">Total monetary amount of the order.</param>
/// <param name="Items">Line items included in the order.</param>
/// <param name="PlacedAt">UTC timestamp when the order was placed.</param>
public record OrderPlacedEvent(
    Guid   OrderId,
    Guid   CustomerId,
    Guid   RestaurantId,
    string CustomerName,
    string CustomerPhone,
    string DeliveryAddress,
    decimal TotalAmount,
    IReadOnlyList<OrderPlacedEvent.OrderLineItem> Items,
    DateTime PlacedAt)
{
    /// <summary>Represents a single line item within an <see cref="OrderPlacedEvent"/>.</summary>
    /// <param name="MenuItemId">Identifier of the menu item ordered.</param>
    /// <param name="Name">Display name of the menu item.</param>
    /// <param name="Quantity">Number of units ordered.</param>
    /// <param name="UnitPrice">Price per unit at the time of ordering.</param>
    public record OrderLineItem(
        Guid    MenuItemId,
        string  Name,
        int     Quantity,
        decimal UnitPrice);
}
