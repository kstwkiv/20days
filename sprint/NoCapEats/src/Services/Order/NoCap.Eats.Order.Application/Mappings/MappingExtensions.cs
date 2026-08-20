// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Order.Application.DTOs;
using NoCap.Eats.Order.Domain.Entities;
using OrderEntity = NoCap.Eats.Order.Domain.Entities.Order;

namespace NoCap.Eats.Order.Application.Mappings;

/// <summary>Static extension methods that map Order domain entities to their corresponding DTOs.</summary>
public static class MappingExtensions
{
    /// <summary>Maps an <see cref="OrderEntity"/> to an <see cref="OrderDto"/> including all line items.</summary>
    public static OrderDto ToDto(this OrderEntity o) => new(
        o.Id, o.CustomerId, o.RestaurantId,
        o.CustomerName, o.CustomerPhone,
        o.DeliveryAddress, o.DeliveryNotes,
        o.Status, o.TotalAmount, o.DeliveryAgentId,
        o.PlacedAt, o.UpdatedAt,
        o.Items.Select(i => i.ToDto()).ToList());

    /// <summary>Maps an <see cref="OrderItem"/> to an <see cref="OrderItemDto"/>.</summary>
    public static OrderItemDto ToDto(this OrderItem i) => new(
        i.Id, i.MenuItemId, i.Name, i.Quantity, i.UnitPrice, i.Subtotal);
}
