// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Delivery.Application.DTOs;
using DeliveryEntity = NoCap.Eats.Delivery.Domain.Entities.Delivery;

namespace NoCap.Eats.Delivery.Application.Mappings;

/// <summary>Static extension methods that map Delivery domain entities to their corresponding DTOs.</summary>
public static class MappingExtensions
{
    /// <summary>Maps a <see cref="DeliveryEntity"/> to a <see cref="DeliveryDto"/>.</summary>
    public static DeliveryDto ToDto(this DeliveryEntity d) => new(
        d.Id, d.OrderId, d.CustomerId, d.RestaurantId, d.AgentId,
        d.CustomerName, d.CustomerPhone, d.DeliveryAddress, d.OrderTotal,
        d.Status, d.CreatedAt, d.PickedUpAt, d.DeliveredAt);
}
