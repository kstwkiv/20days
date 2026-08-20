// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;

namespace NoCap.Eats.Order.Application.Queries.GetRestaurantOrders;

/// <summary>Query to retrieve all orders placed for a specific restaurant.</summary>
/// <param name="RestaurantId">ID of the restaurant whose orders are requested.</param>
public record GetRestaurantOrdersQuery(Guid RestaurantId) : IRequest<IReadOnlyList<OrderDto>>;
