// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;

namespace NoCap.Eats.Order.Application.Queries.GetMyOrders;

/// <summary>Query to retrieve the order history of the authenticated customer.</summary>
/// <param name="CustomerId">ID of the customer whose orders are requested.</param>
public record GetMyOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;
