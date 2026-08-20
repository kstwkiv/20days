// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;

namespace NoCap.Eats.Order.Application.Commands.CancelOrder;

/// <summary>
/// Command for a customer to cancel their own order.
/// Only valid when the order is in Pending or Confirmed status.
/// Publishes <see cref="BuildingBlocks.Events.OrderStatusChangedEvent"/> on success.
/// </summary>
/// <param name="OrderId">ID of the order to cancel.</param>
/// <param name="CustomerId">ID of the customer requesting the cancellation (ownership check).</param>
public record CancelOrderCommand(
    Guid OrderId,
    Guid CustomerId) : IRequest<OrderDto>;
