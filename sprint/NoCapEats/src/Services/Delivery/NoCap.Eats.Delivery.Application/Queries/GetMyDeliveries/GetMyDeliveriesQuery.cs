// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;

namespace NoCap.Eats.Delivery.Application.Queries.GetMyDeliveries;

/// <summary>Query to retrieve all deliveries assigned to the authenticated agent.</summary>
/// <param name="AgentId">ID of the delivery agent whose history is requested.</param>
public record GetMyDeliveriesQuery(Guid AgentId) : IRequest<IReadOnlyList<DeliveryDto>>;
