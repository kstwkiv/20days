// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Notification.Domain.Entities;

namespace NoCap.Eats.Notification.Application.Interfaces;

/// <summary>Data access contract for persisting <see cref="NotificationLog"/> audit records.</summary>
public interface INotificationLogRepository
{
    /// <summary>Adds a new notification log entry to the change tracker.</summary>
    /// <param name="log">The log entry to persist.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(NotificationLog log, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task SaveChangesAsync(CancellationToken ct = default);
}
