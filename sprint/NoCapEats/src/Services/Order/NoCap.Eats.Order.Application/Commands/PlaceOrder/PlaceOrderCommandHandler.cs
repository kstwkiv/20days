// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using MediatR;
using NoCap.Eats.BuildingBlocks.Events;
using NoCap.Eats.Order.Application.DTOs;
using NoCap.Eats.Order.Application.Interfaces;
using NoCap.Eats.Order.Application.Mappings;
using OrderEntity = NoCap.Eats.Order.Domain.Entities.Order;

namespace NoCap.Eats.Order.Application.Commands.PlaceOrder;

/// <summary>
/// Handles <see cref="PlaceOrderCommand"/> by creating the order aggregate,
/// persisting it, then publishing <see cref="OrderPlacedEvent"/> so the
/// Delivery and Notification services can react.
/// </summary>
public class PlaceOrderCommandHandler(
    IOrderRepository repo,
    IPublishEndpoint publisher) : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    /// <summary>Creates, saves, and broadcasts the new order.</summary>
    /// <param name="request">Order details supplied by the customer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="OrderDto"/> representing the newly placed order.</returns>
    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // Project command line items to the tuple form expected by the Order constructor
        var lines = request.Items
            .Select(i => (i.MenuItemId, i.Name, i.Quantity, i.UnitPrice));

        var order = new OrderEntity(
            request.CustomerId,
            request.RestaurantId,
            request.CustomerName,
            request.CustomerPhone,
            request.DeliveryAddress,
            request.DeliveryNotes,
            lines);

        await repo.AddAsync(order, cancellationToken);
        await repo.SaveChangesAsync(cancellationToken);

        // Notify Delivery (creates a Delivery record) and Notification (sends confirmation email)
        await publisher.Publish(new OrderPlacedEvent(
            order.Id,
            order.CustomerId,
            order.RestaurantId,
            order.CustomerName,
            order.CustomerPhone,
            order.DeliveryAddress,
            order.TotalAmount,
            order.Items.Select(i => new OrderPlacedEvent.OrderLineItem(
                i.MenuItemId, i.Name, i.Quantity, i.UnitPrice)).ToList(),
            order.PlacedAt), cancellationToken);

        return order.ToDto();
    }
}
