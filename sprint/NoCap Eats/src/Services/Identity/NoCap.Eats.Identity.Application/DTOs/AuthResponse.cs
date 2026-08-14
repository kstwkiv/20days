namespace NoCap.Eats.Identity.Application.DTOs;

/// <summary>Returned on successful login or token refresh.</summary>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    UserDto User);
