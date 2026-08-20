// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;
using NoCap.Eats.Order.Domain.Enums;

namespace NoCap.Eats.Order.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Command used by the restaurant owner to advance an order through its lifecycle.
/// Also used when a delivery agent updates the order to OutForDelivery or Delivered.
/// Publishes <see cref="BuildingBlocks.Events.OrderStatusChangedEvent"/> on success.
/// </summary>
/// <param name="OrderId">ID of the order to update.</param>
/// <param name="RequesterId">ID of the restaurant or agent driving the transition.</param>
/// <param name="TargetStatus">The desired next status.</param>
public record UpdateOrderStatusCommand(
    Guid        OrderId,
    Guid        RequesterId,
    OrderStatus TargetStatus) : IRequest<OrderDto>;
