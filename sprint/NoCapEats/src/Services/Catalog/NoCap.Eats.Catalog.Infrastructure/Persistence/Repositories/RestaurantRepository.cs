// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Domain.Entities;
using NoCap.Eats.Catalog.Domain.Enums;

namespace NoCap.Eats.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IRestaurantRepository"/>.</summary>
public class RestaurantRepository(CatalogDbContext db) : IRestaurantRepository
{
    /// <inheritdoc/>
    public Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Restaurants.FirstOrDefaultAsync(r => r.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Eagerly loads categories and their items via two-level ThenInclude.</remarks>
    public Task<Restaurant?> GetByIdWithCategoriesAsync(Guid id, CancellationToken ct = default)
        => db.Restaurants
             .Include(r => r.Categories)
             .ThenInclude(c => c.Items)
             .FirstOrDefaultAsync(r => r.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Applies an optional case-insensitive city filter before returning results ordered by name.</remarks>
    public async Task<IReadOnlyList<Restaurant>> GetAllActiveAsync(
        string? city = null, CancellationToken ct = default)
    {
        var query = db.Restaurants
            .Where(r => r.Status == RestaurantStatus.Active);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(r => r.City.ToLower() == city.ToLower());

        return await query.OrderBy(r => r.Name).ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Restaurant>> GetByOwnerAsync(
        Guid ownerId, CancellationToken ct = default)
        => await db.Restaurants
            .Where(r => r.OwnerId == ownerId)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public async Task AddAsync(Restaurant restaurant, CancellationToken ct = default)
        => await db.Restaurants.AddAsync(restaurant, ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
