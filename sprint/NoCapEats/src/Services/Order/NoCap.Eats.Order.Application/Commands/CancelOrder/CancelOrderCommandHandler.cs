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
using NoCap.Eats.Order.Domain.Exceptions;

namespace NoCap.Eats.Order.Application.Commands.CancelOrder;

/// <summary>
/// Handles <see cref="CancelOrderCommand"/> by verifying customer ownership,
/// cancelling the order, and publishing <see cref="OrderStatusChangedEvent"/>.
/// </summary>
public class CancelOrderCommandHandler(
    IOrderRepository repo,
    IPublishEndpoint publisher) : IRequestHandler<CancelOrderCommand, OrderDto>
{
    /// <summary>Cancels the order if the requesting customer owns it and the status allows it.</summary>
    /// <param name="request">Cancel request containing order and customer IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="OrderDto"/> with Cancelled status.</returns>
    /// <exception cref="OrderNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="UnauthorizedOrderAccessException">Thrown when the customer does not own the order.</exception>
    /// <exception cref="OrderNotCancellableException">Thrown when the order is past a cancellable state.</exception>
    public async Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await repo.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        // Ensure only the customer who placed the order can cancel it
        order.GuardCustomer(request.CustomerId);

        var oldStatus = order.Status.ToString();
        order.Cancel();

        await repo.SaveChangesAsync(cancellationToken);

        // Notify downstream services (Notification sends cancellation email)
        await publisher.Publish(new OrderStatusChangedEvent(
            order.Id, order.CustomerId, order.RestaurantId,
            oldStatus, order.Status.ToString(), order.UpdatedAt), cancellationToken);

        return order.ToDto();
    }
}
