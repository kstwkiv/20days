// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Application.Interfaces;

/// <summary>Data access contract for <see cref="MenuItem"/> persistence operations.</summary>
public interface IMenuItemRepository
{
    /// <summary>Returns the menu item with the given ID, or <c>null</c> if not found.</summary>
    Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns all menu items belonging to the specified category.</summary>
    Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
