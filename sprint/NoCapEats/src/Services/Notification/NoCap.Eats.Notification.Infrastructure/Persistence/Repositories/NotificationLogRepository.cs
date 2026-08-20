// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Notification.Application.Interfaces;
using NoCap.Eats.Notification.Domain.Entities;

namespace NoCap.Eats.Notification.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="INotificationLogRepository"/>.</summary>
public class NotificationLogRepository(NotificationDbContext db) : INotificationLogRepository
{
    /// <inheritdoc/>
    public async Task AddAsync(NotificationLog log, CancellationToken ct = default)
        => await db.NotificationLogs.AddAsync(log, ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
