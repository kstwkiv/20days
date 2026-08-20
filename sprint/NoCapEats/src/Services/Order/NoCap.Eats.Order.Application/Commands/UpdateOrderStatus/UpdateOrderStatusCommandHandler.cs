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
using NoCap.Eats.Order.Domain.Enums;
using NoCap.Eats.Order.Domain.Exceptions;

namespace NoCap.Eats.Order.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Handles <see cref="UpdateOrderStatusCommand"/> by invoking the appropriate
/// domain method on the order and publishing <see cref="OrderStatusChangedEvent"/>.
/// Used by restaurant owners to drive the order through its lifecycle.
/// </summary>
public class UpdateOrderStatusCommandHandler(
    IOrderRepository repo,
    IPublishEndpoint publisher) : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    /// <summary>Advances the order to the target status.</summary>
    /// <param name="request">Status update request from the restaurant or delivery agent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="OrderDto"/>.</returns>
    /// <exception cref="OrderNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="InvalidOrderStatusTransitionException">Thrown for unsupported target statuses.</exception>
    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await repo.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        var oldStatus = order.Status.ToString();

        // Delegate to the domain aggregate — each method enforces its own guard
        switch (request.TargetStatus)
        {
            case OrderStatus.Confirmed:      order.Confirm();            break;
            case OrderStatus.Preparing:      order.StartPreparing();     break;
            case OrderStatus.ReadyForPickup: order.MarkReadyForPickup(); break;
            case OrderStatus.Delivered:      order.MarkDelivered();      break;
            case OrderStatus.Rejected:       order.Reject();             break;
            default:
                throw new InvalidOrderStatusTransitionException(
                    order.Status.ToString(), request.TargetStatus.ToString());
        }

        await repo.SaveChangesAsync(cancellationToken);

        // Notify Notification service so customers receive status update emails
        await publisher.Publish(new OrderStatusChangedEvent(
            order.Id,
            order.CustomerId,
            order.RestaurantId,
            oldStatus,
            order.Status.ToString(),
            order.UpdatedAt), cancellationToken);

        return order.ToDto();
    }
}
