// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Order.Domain.Enums;

namespace NoCap.Eats.Order.Application.DTOs;

/// <summary>Read-only projection of an order including all its line items.</summary>
/// <param name="Id">Unique identifier of the order.</param>
/// <param name="CustomerId">Identifier of the customer who placed the order.</param>
/// <param name="RestaurantId">Identifier of the restaurant fulfilling the order.</param>
/// <param name="CustomerName">Full name of the customer.</param>
/// <param name="CustomerPhone">Contact phone number of the customer.</param>
/// <param name="DeliveryAddress">Delivery destination address.</param>
/// <param name="DeliveryNotes">Optional special delivery instructions.</param>
/// <param name="Status">Current lifecycle status of the order.</param>
/// <param name="TotalAmount">Sum of all line item subtotals.</param>
/// <param name="DeliveryAgentId">Identifier of the assigned agent, or <c>null</c>.</param>
/// <param name="PlacedAt">UTC timestamp when the order was placed.</param>
/// <param name="UpdatedAt">UTC timestamp of the most recent status change.</param>
/// <param name="Items">Line items included in the order.</param>
public record OrderDto(
    Guid        Id,
    Guid        CustomerId,
    Guid        RestaurantId,
    string      CustomerName,
    string      CustomerPhone,
    string      DeliveryAddress,
    string?     DeliveryNotes,
    OrderStatus Status,
    decimal     TotalAmount,
    Guid?       DeliveryAgentId,
    DateTime    PlacedAt,
    DateTime    UpdatedAt,
    IReadOnlyList<OrderItemDto> Items);

/// <summary>A single line item within an <see cref="OrderDto"/>.</summary>
/// <param name="Id">Unique identifier of this line item.</param>
/// <param name="MenuItemId">Identifier of the menu item ordered.</param>
/// <param name="Name">Snapshot of the menu item name at order time.</param>
/// <param name="Quantity">Number of units ordered.</param>
/// <param name="UnitPrice">Price per unit at order time.</param>
/// <param name="Subtotal">Computed total: Quantity × UnitPrice.</param>
public record OrderItemDto(
    Guid    Id,
    Guid    MenuItemId,
    string  Name,
    int     Quantity,
    decimal UnitPrice,
    decimal Subtotal);
