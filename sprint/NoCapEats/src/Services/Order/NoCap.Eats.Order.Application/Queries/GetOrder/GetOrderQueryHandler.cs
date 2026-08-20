// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Order.Application.DTOs;
using NoCap.Eats.Order.Application.Interfaces;
using NoCap.Eats.Order.Application.Mappings;
using NoCap.Eats.Order.Domain.Exceptions;

namespace NoCap.Eats.Order.Application.Queries.GetOrder;

/// <summary>
/// Handles <see cref="GetOrderQuery"/> by loading the order with its items
/// and verifying the requester is either the customer or the restaurant.
/// </summary>
public class GetOrderQueryHandler(
    IOrderRepository repo) : IRequestHandler<GetOrderQuery, OrderDto>
{
    /// <summary>Fetches and returns the order if the requester is authorised to view it.</summary>
    /// <param name="request">Query containing the order ID and the requester's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <see cref="OrderDto"/> with all line items.</returns>
    /// <exception cref="OrderNotFoundException">Thrown when the order does not exist.</exception>
    /// <exception cref="UnauthorizedOrderAccessException">Thrown when the requester is neither the customer nor the restaurant.</exception>
    public async Task<OrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await repo.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        // Allow access to the customer who placed it or the restaurant it belongs to
        if (order.CustomerId != request.RequesterId &&
            order.RestaurantId != request.RequesterId)
            throw new UnauthorizedOrderAccessException(request.RequesterId, order.Id);

        return order.ToDto();
    }
}
