// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Application.Mappings;

namespace NoCap.Eats.Delivery.Application.Queries.GetMyDeliveries;

/// <summary>Handles <see cref="GetMyDeliveriesQuery"/> by returning the agent's delivery history.</summary>
public class GetMyDeliveriesQueryHandler(
    IDeliveryRepository repo) : IRequestHandler<GetMyDeliveriesQuery, IReadOnlyList<DeliveryDto>>
{
    /// <summary>Returns all deliveries assigned to the authenticated agent, most recent first.</summary>
    /// <param name="request">Query containing the agent's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="DeliveryDto"/> records for the agent.</returns>
    public async Task<IReadOnlyList<DeliveryDto>> Handle(
        GetMyDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var deliveries = await repo.GetByAgentAsync(request.AgentId, cancellationToken);
        return deliveries.Select(d => d.ToDto()).ToList();
    }
}
