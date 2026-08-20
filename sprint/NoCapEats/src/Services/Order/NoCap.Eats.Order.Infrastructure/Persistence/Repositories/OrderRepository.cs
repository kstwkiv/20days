// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Order.Application.Interfaces;

namespace NoCap.Eats.Order.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IOrderRepository"/>.
/// All queries that need line items use <c>Include(o => o.Items)</c> for eager loading.
/// </summary>
public class OrderRepository(OrderDbContext db) : IOrderRepository
{
    /// <inheritdoc/>
    public Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Eagerly loads <c>Items</c> so the aggregate is fully hydrated for domain operations.</remarks>
    public Task<Domain.Entities.Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default)
        => db.Orders
             .Include(o => o.Items)
             .FirstOrDefaultAsync(o => o.Id == id, ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Domain.Entities.Order>> GetByCustomerAsync(
        Guid customerId, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.PlacedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Domain.Entities.Order>> GetByRestaurantAsync(
        Guid restaurantId, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Items)
            .Where(o => o.RestaurantId == restaurantId)
            .OrderByDescending(o => o.PlacedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task AddAsync(Domain.Entities.Order order, CancellationToken ct = default)
        => await db.Orders.AddAsync(order, ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
