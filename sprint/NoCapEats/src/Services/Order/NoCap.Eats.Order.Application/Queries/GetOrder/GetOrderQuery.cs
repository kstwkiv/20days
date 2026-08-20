// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;

namespace NoCap.Eats.Order.Application.Queries.GetOrder;

/// <summary>
/// Query to retrieve a single order with its line items.
/// Access is restricted to the customer who placed it or the restaurant it belongs to.
/// </summary>
/// <param name="OrderId">ID of the order to fetch.</param>
/// <param name="RequesterId">ID of the user requesting access (customer or restaurant).</param>
public record GetOrderQuery(Guid OrderId, Guid RequesterId) : IRequest<OrderDto>;
