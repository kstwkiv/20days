// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;
using RefreshTokenEntity = NoCap.Eats.Identity.Domain.Entities.RefreshToken;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

/// <summary>
/// Handles <see cref="RefreshTokenCommand"/> using refresh token rotation:
/// the submitted token is revoked and a brand-new pair of tokens is issued.
/// This prevents replay attacks with stolen refresh tokens.
/// </summary>
public class RefreshTokenCommandHandler(
    UserManager<AppUser>       userManager,
    IUserRepository            userRepository,
    IRefreshTokenRepository    refreshTokenRepo,
    ITokenService              tokenService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    /// <summary>
    /// Rotates the refresh token and issues a fresh access token.
    /// </summary>
    /// <param name="request">Contains the user ID and the raw refresh token to rotate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>New <see cref="AuthResponse"/> with rotated tokens.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the user ID does not exist.</exception>
    /// <exception cref="AccountDeactivatedException">Thrown when the account has been deactivated.</exception>
    /// <exception cref="InvalidRefreshTokenException">Thrown when no matching active token is found.</exception>
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        if (!user.IsActive)
            throw new AccountDeactivatedException();

        // Fetch all active (non-revoked, non-expired) tokens for this user
        var activeTokens = await refreshTokenRepo.GetActiveByUserIdAsync(user.Id, cancellationToken);

        // BCrypt.Verify each hash until we find the one matching the submitted raw token
        var matchedToken = activeTokens.FirstOrDefault(t =>
            tokenService.VerifyRefreshToken(request.RawRefreshToken, t.TokenHash));

        if (matchedToken is null)
            throw new InvalidRefreshTokenException();

        // Rotation: invalidate the used token before issuing a new one
        matchedToken.Revoke();

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;

        var (newAccessToken, expiresAt) = tokenService.GenerateAccessToken(user, role);

        // Issue a brand-new refresh token; only the hash is persisted
        var newRaw    = tokenService.GenerateRawRefreshToken();
        var newHashed = tokenService.HashRefreshToken(newRaw);
        var newToken  = new RefreshTokenEntity(user.Id, newHashed, DateTime.UtcNow.AddDays(30));

        await refreshTokenRepo.AddAsync(newToken, cancellationToken);
        await refreshTokenRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            newAccessToken,
            newRaw,
            expiresAt,
            new UserDto(user.Id, user.FullName, user.Email!, user.MobileNumber, role,
                        user.IsActive, user.CreatedAt, user.LastLoginAt));
    }
}
