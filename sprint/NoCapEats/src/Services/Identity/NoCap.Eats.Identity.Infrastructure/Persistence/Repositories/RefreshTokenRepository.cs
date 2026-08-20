// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRefreshTokenRepository"/>.
/// All write operations are tracked by the same <see cref="IdentityDbContext"/>
/// instance, so a single <c>SaveChangesAsync</c> call commits everything atomically.
/// </summary>
public class RefreshTokenRepository(IdentityDbContext db) : IRefreshTokenRepository
{
    /// <inheritdoc/>
    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
        => await db.RefreshTokens.AddAsync(token, ct);

    /// <inheritdoc/>
    /// <remarks>Filters out revoked and expired tokens server-side to avoid loading stale records.</remarks>
    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId, CancellationToken ct = default)
        => await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

    /// <inheritdoc/>
    /// <remarks>
    /// Loads all non-revoked tokens into memory then calls <see cref="RefreshToken.Revoke"/>
    /// on each, relying on EF Core change tracking to persist the updates.
    /// </remarks>
    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var active = await db.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in active)
            token.Revoke();
    }

    /// <inheritdoc/>
    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
