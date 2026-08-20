// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Application.Mappings;
using NoCap.Eats.Delivery.Domain.Enums;
using NoCap.Eats.Delivery.Domain.Exceptions;

namespace NoCap.Eats.Delivery.Application.Commands.UpdateDeliveryStatus;

/// <summary>
/// Handles <see cref="UpdateDeliveryStatusCommand"/> by invoking the appropriate
/// domain method on the delivery aggregate (PickedUp, Delivered, or Failed).
/// </summary>
public class UpdateDeliveryStatusCommandHandler(
    IDeliveryRepository repo) : IRequestHandler<UpdateDeliveryStatusCommand, DeliveryDto>
{
    /// <summary>Advances the delivery to the target status.</summary>
    /// <param name="request">Status update request from the assigned agent.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="DeliveryDto"/>.</returns>
    /// <exception cref="DeliveryNotFoundException">Thrown when the delivery does not exist.</exception>
    /// <exception cref="UnauthorizedDeliveryAccessException">Thrown when the agent does not own the delivery.</exception>
    /// <exception cref="InvalidDeliveryStatusTransitionException">Thrown for unsupported target statuses.</exception>
    public async Task<DeliveryDto> Handle(
        UpdateDeliveryStatusCommand request, CancellationToken cancellationToken)
    {
        var delivery = await repo.GetByIdAsync(request.DeliveryId, cancellationToken)
            ?? throw new DeliveryNotFoundException(request.DeliveryId);

        // Delegate to the domain aggregate; each method enforces agent ownership and valid transitions
        switch (request.TargetStatus)
        {
            case DeliveryStatus.PickedUp:  delivery.MarkPickedUp(request.AgentId);  break;
            case DeliveryStatus.Delivered: delivery.MarkDelivered(request.AgentId); break;
            case DeliveryStatus.Failed:    delivery.MarkFailed(request.AgentId);    break;
            default:
                throw new InvalidDeliveryStatusTransitionException(
                    delivery.Status.ToString(), request.TargetStatus.ToString());
        }

        await repo.SaveChangesAsync(cancellationToken);
        return delivery.ToDto();
    }
}
