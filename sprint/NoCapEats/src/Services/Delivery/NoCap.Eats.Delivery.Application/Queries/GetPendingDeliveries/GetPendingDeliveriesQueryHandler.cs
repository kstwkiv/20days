// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Application.Mappings;

namespace NoCap.Eats.Delivery.Application.Queries.GetPendingDeliveries;

/// <summary>
/// Handles <see cref="GetPendingDeliveriesQuery"/> by returning all deliveries
/// that are in Pending status and waiting for an agent to accept them.
/// </summary>
public class GetPendingDeliveriesQueryHandler(
    IDeliveryRepository repo) : IRequestHandler<GetPendingDeliveriesQuery, IReadOnlyList<DeliveryDto>>
{
    /// <summary>Returns the agent job board — all unassigned pending deliveries.</summary>
    /// <param name="request">Query with no parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="DeliveryDto"/> records ordered by creation time ascending.</returns>
    public async Task<IReadOnlyList<DeliveryDto>> Handle(
        GetPendingDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var deliveries = await repo.GetPendingAsync(cancellationToken);
        return deliveries.Select(d => d.ToDto()).ToList();
    }
}
