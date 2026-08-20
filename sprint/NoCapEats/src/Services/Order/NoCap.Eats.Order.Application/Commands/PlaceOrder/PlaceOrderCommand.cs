// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;

namespace NoCap.Eats.Order.Application.Commands.PlaceOrder;

/// <summary>
/// Command to place a new food order on behalf of a customer.
/// Publishes <see cref="BuildingBlocks.Events.OrderPlacedEvent"/> on success.
/// </summary>
/// <param name="CustomerId">ID of the customer placing the order.</param>
/// <param name="RestaurantId">ID of the restaurant to order from.</param>
/// <param name="CustomerName">Full name of the customer for delivery.</param>
/// <param name="CustomerPhone">Contact phone number of the customer.</param>
/// <param name="DeliveryAddress">Street address for delivery.</param>
/// <param name="DeliveryNotes">Optional special delivery instructions.</param>
/// <param name="Items">One or more line items to order.</param>
public record PlaceOrderCommand(
    Guid   CustomerId,
    Guid   RestaurantId,
    string CustomerName,
    string CustomerPhone,
    string DeliveryAddress,
    string? DeliveryNotes,
    IReadOnlyList<PlaceOrderCommand.OrderLine> Items) : IRequest<OrderDto>
{
    /// <summary>A single line item within a <see cref="PlaceOrderCommand"/>.</summary>
    /// <param name="MenuItemId">ID of the menu item to order.</param>
    /// <param name="Name">Display name of the item (snapshotted at order time).</param>
    /// <param name="Quantity">Number of units to order.</param>
    /// <param name="UnitPrice">Price per unit (snapshotted at order time).</param>
    public record OrderLine(
        Guid    MenuItemId,
        string  Name,
        int     Quantity,
        decimal UnitPrice);
}
