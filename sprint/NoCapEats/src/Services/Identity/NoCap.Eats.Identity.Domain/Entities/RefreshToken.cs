// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Domain.Entities;

/// <summary>
/// Persisted refresh token — stores a bcrypt hash, not the raw token.
/// </summary>
public class RefreshToken
{
    /// <summary>Unique identifier of this refresh token record.</summary>
    public Guid   Id          { get; private set; } = Guid.NewGuid();

    /// <summary>Identifier of the user this token belongs to.</summary>
    public Guid   UserId      { get; private set; }

    /// <summary>Bcrypt hash of the raw refresh token string.</summary>
    public string TokenHash   { get; private set; } = default!;

    /// <summary>UTC timestamp after which this token is no longer valid.</summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>Indicates whether this token has been explicitly revoked.</summary>
    public bool   IsRevoked   { get; private set; }

    /// <summary>UTC timestamp when this token record was created.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected RefreshToken() { }

    /// <summary>Creates a new <see cref="RefreshToken"/> for the specified user.</summary>
    /// <param name="userId">Identifier of the owning user.</param>
    /// <param name="tokenHash">Bcrypt hash of the raw refresh token.</param>
    /// <param name="expiresAt">UTC expiry time for this token.</param>
    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId    = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    /// <summary>Returns <c>true</c> when the token's expiry time has passed.</summary>
    public bool IsExpired  => DateTime.UtcNow >= ExpiresAt;

    /// <summary>Returns <c>true</c> when the token is neither revoked nor expired.</summary>
    public bool IsActive   => !IsRevoked && !IsExpired;

    /// <summary>Marks this token as revoked, preventing further use.</summary>
    public void Revoke() => IsRevoked = true;
}
