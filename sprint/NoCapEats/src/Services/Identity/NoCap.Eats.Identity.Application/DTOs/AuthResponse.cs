// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

namespace NoCap.Eats.Identity.Application.DTOs;

/// <summary>Returned on successful login or token refresh, containing both tokens and the user profile.</summary>
/// <param name="AccessToken">Signed JWT bearer token used to authenticate API requests.</param>
/// <param name="RefreshToken">Raw opaque token used to obtain a new access token after expiry.</param>
/// <param name="AccessTokenExpiresAt">UTC timestamp when the access token expires.</param>
/// <param name="User">Profile of the authenticated user.</param>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    UserDto User);
