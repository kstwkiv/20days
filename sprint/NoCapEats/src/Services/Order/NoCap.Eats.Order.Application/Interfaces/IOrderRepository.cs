// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using OrderEntity = NoCap.Eats.Order.Domain.Entities.Order;

namespace NoCap.Eats.Order.Application.Interfaces;

/// <summary>Data access contract for <see cref="OrderEntity"/> persistence operations.</summary>
public interface IOrderRepository
{
    /// <summary>Returns the order with the given ID, or <c>null</c> if not found.</summary>
    Task<OrderEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the order with its line items eagerly loaded, or <c>null</c>.</summary>
    Task<OrderEntity?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns all orders placed by the given customer, most recent first.</summary>
    Task<IReadOnlyList<OrderEntity>> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);

    /// <summary>Returns all orders for the given restaurant, most recent first.</summary>
    Task<IReadOnlyList<OrderEntity>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);

    /// <summary>Adds a new order to the change tracker for insertion on the next save.</summary>
    Task AddAsync(OrderEntity order, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
