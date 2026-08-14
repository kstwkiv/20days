using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;
using RefreshTokenEntity = NoCap.Eats.Identity.Domain.Entities.RefreshToken;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    UserManager<AppUser>       userManager,
    IUserRepository            userRepository,
    IRefreshTokenRepository    refreshTokenRepo,
    ITokenService              tokenService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        if (!user.IsActive)
            throw new AccountDeactivatedException();

        var activeTokens = await refreshTokenRepo.GetActiveByUserIdAsync(user.Id, cancellationToken);

        var matchedToken = activeTokens.FirstOrDefault(t =>
            tokenService.VerifyRefreshToken(request.RawRefreshToken, t.TokenHash));

        if (matchedToken is null)
            throw new InvalidRefreshTokenException();

        // Rotate: revoke old, issue new
        matchedToken.Revoke();

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;

        var (newAccessToken, expiresAt) = tokenService.GenerateAccessToken(user, role);

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
