// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;

namespace NoCap.Eats.Delivery.Application.Queries.GetPendingDeliveries;

/// <summary>Returns all deliveries awaiting agent assignment.</summary>
public record GetPendingDeliveriesQuery : IRequest<IReadOnlyList<DeliveryDto>>;
