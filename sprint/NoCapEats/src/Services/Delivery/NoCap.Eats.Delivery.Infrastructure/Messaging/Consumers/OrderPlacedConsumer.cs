// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using Microsoft.Extensions.Logging;
using NoCap.Eats.BuildingBlocks.Events;
using NoCap.Eats.Delivery.Application.Interfaces;
using DeliveryEntity = NoCap.Eats.Delivery.Domain.Entities.Delivery;

namespace NoCap.Eats.Delivery.Infrastructure.Messaging.Consumers;

/// <summary>
/// MassTransit consumer that reacts to <see cref="OrderPlacedEvent"/>.
/// Creates a pending <see cref="DeliveryEntity"/> record so the delivery
/// becomes visible to agents on their job board.
/// The operation is idempotent — duplicate events are safely ignored.
/// </summary>
public class OrderPlacedConsumer(
    IDeliveryRepository          repo,
    ILogger<OrderPlacedConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    /// <summary>
    /// Creates a pending delivery for the placed order if one does not already exist.
    /// </summary>
    /// <param name="context">MassTransit consume context wrapping the <see cref="OrderPlacedEvent"/>.</param>
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var ev = context.Message;

        // Idempotency guard — re-delivered events must not create duplicate deliveries
        var existing = await repo.GetByOrderIdAsync(ev.OrderId, context.CancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Delivery for order {OrderId} already exists, skipping.", ev.OrderId);
            return;
        }

        var delivery = new DeliveryEntity(
            ev.OrderId,
            ev.CustomerId,
            ev.RestaurantId,
            ev.CustomerName,
            ev.CustomerPhone,
            ev.DeliveryAddress,
            ev.TotalAmount);

        await repo.AddAsync(delivery, context.CancellationToken);
        await repo.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation(
            "Created delivery {DeliveryId} for order {OrderId}", delivery.Id, ev.OrderId);
    }
}
