// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Infrastructure.Settings;

namespace NoCap.Eats.Identity.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="ITokenService"/> that produces HMAC-SHA256 signed JWTs
/// and BCrypt-hashed refresh tokens using settings from <see cref="JwtSettings"/>.
/// </summary>
public class TokenService(IOptions<JwtSettings> jwtOptions) : ITokenService
{
    /// <summary>Cached JWT settings resolved from DI options.</summary>
    private readonly JwtSettings _jwt = jwtOptions.Value;

    /// <inheritdoc/>
    /// <remarks>
    /// Embeds sub, email, name, jti, and role claims.
    /// Token is signed with HMAC-SHA256 using the configured secret.
    /// </remarks>
    public (string Token, DateTime ExpiresAt) GenerateAccessToken(AppUser user, string role)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes);

        // Standard JWT claims plus a role claim for authorization
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name,  user.FullName),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()), // unique token ID
            new Claim(ClaimTypes.Role,               role),
        };

        // Build symmetric key from the configured secret
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _jwt.Issuer,
            audience:           _jwt.Audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    /// <inheritdoc/>
    /// <remarks>Uses <see cref="RandomNumberGenerator"/> for cryptographically secure randomness.</remarks>
    public string GenerateRawRefreshToken()
    {
        // 64 random bytes encoded as Base64 gives a 88-character URL-safe string
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    /// <inheritdoc/>
    /// <remarks>Work factor 11 balances security and performance (≈300ms on modern hardware).</remarks>
    public string HashRefreshToken(string rawToken)
        => BCrypt.Net.BCrypt.HashPassword(rawToken, workFactor: 11);

    /// <inheritdoc/>
    public bool VerifyRefreshToken(string rawToken, string storedHash)
        => BCrypt.Net.BCrypt.Verify(rawToken, storedHash);
}
