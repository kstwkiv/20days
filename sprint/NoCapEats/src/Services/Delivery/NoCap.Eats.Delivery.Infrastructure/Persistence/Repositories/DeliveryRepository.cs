// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Delivery.Application.Interfaces;
using NoCap.Eats.Delivery.Domain.Enums;
using DeliveryEntity = NoCap.Eats.Delivery.Domain.Entities.Delivery;

namespace NoCap.Eats.Delivery.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IDeliveryRepository"/>.</summary>
public class DeliveryRepository(DeliveryDbContext db) : IDeliveryRepository
{
    /// <inheritdoc/>
    public Task<DeliveryEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Deliveries.FirstOrDefaultAsync(d => d.Id == id, ct);

    /// <inheritdoc/>
    public Task<DeliveryEntity?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        => db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == orderId, ct);

    /// <inheritdoc/>
    /// <remarks>Orders results by <c>CreatedAt</c> ascending so oldest jobs appear first.</remarks>
    public async Task<IReadOnlyList<DeliveryEntity>> GetPendingAsync(CancellationToken ct = default)
        => await db.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    /// <remarks>Orders results by <c>CreatedAt</c> descending so most recent work appears first.</remarks>
    public async Task<IReadOnlyList<DeliveryEntity>> GetByAgentAsync(
        Guid agentId, CancellationToken ct = default)
        => await db.Deliveries
            .Where(d => d.AgentId == agentId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task AddAsync(DeliveryEntity delivery, CancellationToken ct = default)
        => await db.Deliveries.AddAsync(delivery, ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
