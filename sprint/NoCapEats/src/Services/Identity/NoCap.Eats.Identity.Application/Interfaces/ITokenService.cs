// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Application.Interfaces;

public interface ITokenService
{
    /// <summary>Generates a signed JWT access token for the given user and role.</summary>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(AppUser user, string role);

    /// <summary>Generates a cryptographically random raw refresh token string.</summary>
    string GenerateRawRefreshToken();

    /// <summary>Hashes a raw refresh token for safe storage.</summary>
    string HashRefreshToken(string rawToken);

    /// <summary>Verifies a raw token against its stored hash.</summary>
    bool VerifyRefreshToken(string rawToken, string storedHash);
}
