// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using DeliveryEntity = NoCap.Eats.Delivery.Domain.Entities.Delivery;

namespace NoCap.Eats.Delivery.Application.Interfaces;

/// <summary>Data access contract for <see cref="DeliveryEntity"/> persistence operations.</summary>
public interface IDeliveryRepository
{
    /// <summary>Returns the delivery with the given ID, or <c>null</c> if not found.</summary>
    Task<DeliveryEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the delivery associated with the given order ID, or <c>null</c>.</summary>
    Task<DeliveryEntity?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);

    /// <summary>Returns all deliveries in Pending status, ordered by creation time (oldest first).</summary>
    Task<IReadOnlyList<DeliveryEntity>> GetPendingAsync(CancellationToken ct = default);

    /// <summary>Returns all deliveries assigned to the given agent, most recent first.</summary>
    Task<IReadOnlyList<DeliveryEntity>> GetByAgentAsync(Guid agentId, CancellationToken ct = default);

    /// <summary>Adds a new delivery record to the change tracker.</summary>
    Task AddAsync(DeliveryEntity delivery, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
