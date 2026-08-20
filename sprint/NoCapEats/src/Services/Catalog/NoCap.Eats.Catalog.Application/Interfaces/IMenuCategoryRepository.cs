// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Application.Interfaces;

/// <summary>Data access contract for <see cref="MenuCategory"/> persistence operations.</summary>
public interface IMenuCategoryRepository
{
    /// <summary>Returns the category with the given ID, or <c>null</c> if not found.</summary>
    Task<MenuCategory?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the category with its items eagerly loaded, or <c>null</c> if not found.</summary>
    Task<MenuCategory?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns all active categories for the given restaurant, ordered by SortOrder.</summary>
    Task<IReadOnlyList<MenuCategory>> GetByRestaurantAsync(Guid restaurantId, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
