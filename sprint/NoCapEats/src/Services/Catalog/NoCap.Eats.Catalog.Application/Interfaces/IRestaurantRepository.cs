// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Application.Interfaces;

/// <summary>Data access contract for <see cref="Restaurant"/> persistence operations.</summary>
public interface IRestaurantRepository
{
    /// <summary>Returns the restaurant with the given ID, or <c>null</c> if not found.</summary>
    Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns the restaurant with its full category and item graph eagerly loaded, or <c>null</c>.</summary>
    Task<Restaurant?> GetByIdWithCategoriesAsync(Guid id, CancellationToken ct = default);

    /// <summary>Returns all Active restaurants, optionally filtered to a specific city.</summary>
    /// <param name="city">Optional city name to filter by (case-insensitive). Pass <c>null</c> to return all cities.</param>
    Task<IReadOnlyList<Restaurant>> GetAllActiveAsync(string? city = null, CancellationToken ct = default);

    /// <summary>Returns all restaurants owned by the specified owner, regardless of status.</summary>
    Task<IReadOnlyList<Restaurant>> GetByOwnerAsync(Guid ownerId, CancellationToken ct = default);

    /// <summary>Adds a new restaurant to the change tracker for insertion on the next save.</summary>
    Task AddAsync(Restaurant restaurant, CancellationToken ct = default);

    /// <summary>Flushes all tracked changes to the database.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
