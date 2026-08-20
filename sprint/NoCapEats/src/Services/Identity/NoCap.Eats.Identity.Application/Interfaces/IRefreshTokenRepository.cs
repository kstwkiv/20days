// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Application.Interfaces;

/// <summary>Data access contract for persisted <see cref="RefreshToken"/> records.</summary>
public interface IRefreshTokenRepository
{
    /// <summary>Persists a new refresh token record to the store.</summary>
    /// <param name="token">The refresh token to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    /// <summary>Returns all non-revoked, non-expired tokens for the specified user.</summary>
    /// <param name="userId">The user whose active tokens are requested.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Marks every non-revoked token for the specified user as revoked (logout).</summary>
    /// <param name="userId">The user whose tokens should be revoked.</param>
    /// <param name="ct">Cancellation token.</param>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>Flushes pending changes to the underlying data store.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task SaveChangesAsync(CancellationToken ct = default);
}
