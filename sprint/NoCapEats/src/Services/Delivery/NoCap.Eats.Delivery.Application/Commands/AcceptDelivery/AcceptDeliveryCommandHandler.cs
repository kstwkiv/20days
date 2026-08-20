// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Application.Mappings;
using NoCap.Eats.Delivery.Domain.Exceptions;

namespace NoCap.Eats.Delivery.Application.Commands.AcceptDelivery;

/// <summary>
/// Handles <see cref="AcceptDeliveryCommand"/> by assigning the requesting agent
/// to the delivery and transitioning it from Pending to Assigned.
/// </summary>
public class AcceptDeliveryCommandHandler(
    IDeliveryRepository repo) : IRequestHandler<AcceptDeliveryCommand, DeliveryDto>
{
    /// <summary>Assigns the agent and saves the updated delivery record.</summary>
    /// <param name="request">Command containing the delivery ID and agent ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="DeliveryDto"/> with Assigned status.</returns>
    /// <exception cref="DeliveryNotFoundException">Thrown when the delivery does not exist.</exception>
    /// <exception cref="DeliveryAlreadyAssignedException">Thrown when the delivery is already assigned.</exception>
    public async Task<DeliveryDto> Handle(
        AcceptDeliveryCommand request, CancellationToken cancellationToken)
    {
        var delivery = await repo.GetByIdAsync(request.DeliveryId, cancellationToken)
            ?? throw new DeliveryNotFoundException(request.DeliveryId);

        delivery.AssignAgent(request.AgentId);

        await repo.SaveChangesAsync(cancellationToken);
        return delivery.ToDto();
    }
}
