// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;
using NoCap.Eats.Order.Application.Interfaces;
using NoCap.Eats.Order.Application.Mappings;

namespace NoCap.Eats.Order.Application.Queries.GetMyOrders;

/// <summary>Handles <see cref="GetMyOrdersQuery"/> by returning the customer's full order history.</summary>
public class GetMyOrdersQueryHandler(
    IOrderRepository repo) : IRequestHandler<GetMyOrdersQuery, IReadOnlyList<OrderDto>>
{
    /// <summary>Returns all orders for the authenticated customer, most recent first.</summary>
    /// <param name="request">Query containing the customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of <see cref="OrderDto"/> records ordered by placement date descending.</returns>
    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await repo.GetByCustomerAsync(request.CustomerId, cancellationToken);
        return orders.Select(o => o.ToDto()).ToList();
    }
}
