using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;
using RefreshTokenEntity = NoCap.Eats.Identity.Domain.Entities.RefreshToken;

namespace NoCap.Eats.Identity.Application.Commands.Login;

public class LoginCommandHandler(
    UserManager<AppUser>       userManager,
    IUserRepository            userRepository,
    IRefreshTokenRepository    refreshTokenRepo,
    ITokenService              tokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDeactivatedException();

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new InvalidCredentialsException();

        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;

        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, role);

        var rawRefreshToken  = tokenService.GenerateRawRefreshToken();
        var hashedToken      = tokenService.HashRefreshToken(rawRefreshToken);
        var refreshToken     = new RefreshTokenEntity(user.Id, hashedToken, DateTime.UtcNow.AddDays(30));

        await refreshTokenRepo.AddAsync(refreshToken, cancellationToken);

        user.RecordLogin();
        await userManager.UpdateAsync(user);

        await refreshTokenRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            rawRefreshToken,
            expiresAt,
            new UserDto(user.Id, user.FullName, user.Email!, user.MobileNumber, role,
                        user.IsActive, user.CreatedAt, user.LastLoginAt));
    }
}
