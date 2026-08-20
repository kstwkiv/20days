// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Domain.Entities;

namespace NoCap.Eats.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IMenuItemRepository"/>.</summary>
public class MenuItemRepository(CatalogDbContext db) : IMenuItemRepository
{
    /// <inheritdoc/>
    public Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.MenuItems.FirstOrDefaultAsync(i => i.Id == id, ct);

    /// <inheritdoc/>
    /// <remarks>Returns items ordered alphabetically by name.</remarks>
    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(
        Guid categoryId, CancellationToken ct = default)
        => await db.MenuItems
            .Where(i => i.CategoryId == categoryId)
            .OrderBy(i => i.Name)
            .ToListAsync(ct);

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
