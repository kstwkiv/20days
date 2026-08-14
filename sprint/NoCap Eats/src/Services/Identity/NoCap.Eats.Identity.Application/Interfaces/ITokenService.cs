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
