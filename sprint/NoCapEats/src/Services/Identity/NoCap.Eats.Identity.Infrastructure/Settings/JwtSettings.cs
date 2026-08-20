// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Infrastructure.Settings;

/// <summary>
/// Strongly-typed configuration options for JWT token generation.
/// Bound from the "Jwt" section of appsettings.json.
/// </summary>
public class JwtSettings
{
    /// <summary>The configuration section key used to bind this settings object.</summary>
    public const string SectionName = "Jwt";

    /// <summary>HMAC-SHA256 signing secret — must be at least 32 characters in production.</summary>
    public string Secret { get; init; } = default!;

    /// <summary>Token issuer claim (iss) — identifies this service as the token issuer.</summary>
    public string Issuer { get; init; } = default!;

    /// <summary>Token audience claim (aud) — identifies the intended consumers of the token.</summary>
    public string Audience { get; init; } = default!;

    /// <summary>Number of minutes before the access token expires. Defaults to 15 minutes.</summary>
    public int AccessTokenExpiryMinutes { get; init; } = 15;
}
