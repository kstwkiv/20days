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

namespace NoCap.Eats.Identity.Application.Commands.Login;

/// <summary>
/// Handles <see cref="LoginCommand"/> by verifying credentials, issuing a JWT
/// access token, generating a BCrypt-hashed refresh token, and recording the login.
/// </summary>
public class LoginCommandHandler(
    UserManager<AppUser>       userManager,
    IUserRepository            userRepository,
    IRefreshTokenRepository    refreshTokenRepo,
    ITokenService              tokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    /// <summary>
    /// Validates the email and password, then issues access and refresh tokens.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="AuthResponse"/> containing both tokens and the user profile.</returns>
    /// <exception cref="InvalidCredentialsException">Thrown when email or password is incorrect.</exception>
    /// <exception cref="AccountDeactivatedException">Thrown when the account has been deactivated.</exception>
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Look up by normalised email; return generic error to avoid user enumeration
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDeactivatedException();

        // ASP.NET Core Identity handles the BCrypt password comparison
        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new InvalidCredentialsException();

        // Retrieve the user's single role for embedding in the JWT
        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, role);

        // Generate a random raw token, store only its BCrypt hash
        var rawRefreshToken  = tokenService.GenerateRawRefreshToken();
        var hashedToken      = tokenService.HashRefreshToken(rawRefreshToken);
        var refreshToken     = new RefreshTokenEntity(user.Id, hashedToken, DateTime.UtcNow.AddDays(30));

        await refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

        // Stamp LastLoginAt before persisting
        user.RecordLogin();
        await userManager.UpdateAsync(user);

        await refreshTokenRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            rawRefreshToken,   // raw token returned to client; hash stays in DB
            expiresAt,
            new UserDto(user.Id, user.FullName, user.Email!, user.MobileNumber, role,
                        user.IsActive, user.CreatedAt, user.LastLoginAt));
    }
}
