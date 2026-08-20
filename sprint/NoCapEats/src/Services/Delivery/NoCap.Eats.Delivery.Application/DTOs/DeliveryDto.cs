// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Delivery.Domain.Enums;

namespace NoCap.Eats.Delivery.Application.DTOs;

/// <summary>Read-only projection of a delivery job returned to API callers.</summary>
/// <param name="Id">Unique identifier of the delivery record.</param>
/// <param name="OrderId">Identifier of the order this delivery is for.</param>
/// <param name="CustomerId">Identifier of the customer who placed the order.</param>
/// <param name="RestaurantId">Identifier of the restaurant the food is collected from.</param>
/// <param name="AgentId">Identifier of the assigned delivery agent, or <c>null</c> if unassigned.</param>
/// <param name="CustomerName">Full name of the customer for delivery confirmation.</param>
/// <param name="CustomerPhone">Contact phone of the customer.</param>
/// <param name="DeliveryAddress">Street address for delivery.</param>
/// <param name="OrderTotal">Total monetary value of the order.</param>
/// <param name="Status">Current lifecycle status of this delivery.</param>
/// <param name="CreatedAt">UTC timestamp when the delivery record was created.</param>
/// <param name="PickedUpAt">UTC timestamp when the agent picked up the food, or <c>null</c>.</param>
/// <param name="DeliveredAt">UTC timestamp when the order was handed to the customer, or <c>null</c>.</param>
public record DeliveryDto(
    Guid           Id,
    Guid           OrderId,
    Guid           CustomerId,
    Guid           RestaurantId,
    Guid?          AgentId,
    string         CustomerName,
    string         CustomerPhone,
    string         DeliveryAddress,
    decimal        OrderTotal,
    DeliveryStatus Status,
    DateTime       CreatedAt,
    DateTime?      PickedUpAt,
    DateTime?      DeliveredAt);
