// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IMenuCategoryRepository"/>.</summary>
public class MenuCategoryRepository(CatalogDbContext db) : IMenuCategoryRepository
{
    /// <inheritdoc/>
    public Task<MenuCategory?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.MenuCategories.FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Eagerly loads <c>Items</c> for use in detail views.</remarks>
    public Task<MenuCategory?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default)
        => db.MenuCategories
             .Include(c => c.Items)
             .FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Returns only active categories, sorted by <c>SortOrder</c> for display ordering.</remarks>
    public async Task<IReadOnlyList<MenuCategory>> GetByRestaurantAsync(
        Guid restaurantId, CancellationToken ct = default)
        => await db.MenuCategories
            .Include(c => c.Items)
            .Where(c => c.RestaurantId == restaurantId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
