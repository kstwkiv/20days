// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.Commands.Login;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;
using NSubstitute;

namespace NoCap.Eats.Identity.Tests.Commands;

public class LoginCommandHandlerTests
{
    private readonly UserManager<AppUser>  _userManager;
    private readonly IUserRepository       _userRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly ITokenService         _tokenService;
    private readonly LoginCommandHandler   _sut;

    public LoginCommandHandlerTests()
    {
        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager  = Substitute.For<UserManager<AppUser>>(
            store, null, null, null, null, null, null, null, null);
        _userRepo     = Substitute.For<IUserRepository>();
        _refreshRepo  = Substitute.For<IRefreshTokenRepository>();
        _tokenService = Substitute.For<ITokenService>();
        _sut          = new LoginCommandHandler(
            _userManager, _userRepo, _refreshRepo, _tokenService);
    }

    private static AppUser MakeUser(string email = "priya@test.com") =>
        new("priya Customer", email, "9934567890");

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user    = MakeUser();
        var command = new LoginCommand("priya@test.com", "Password1!");
        var expiry  = DateTime.UtcNow.AddMinutes(15);

        _userRepo.GetByEmailAsync("priya@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Password1!").Returns(true);
        _userManager.GetRolesAsync(user).Returns(["Customer"]);
        _tokenService.GenerateAccessToken(user, "Customer").Returns(("jwt-token", expiry));
        _tokenService.GenerateRawRefreshToken().Returns("raw-refresh");
        _tokenService.HashRefreshToken("raw-refresh").Returns("hashed-refresh");
        _userManager.UpdateAsync(user).Returns(IdentityResult.Success);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("jwt-token");
        result.RefreshToken.Should().Be("raw-refresh");
        result.AccessTokenExpiresAt.Should().Be(expiry);
        result.User.Email.Should().Be("priya@test.com");
        result.User.Role.Should().Be("Customer");
    }

    // ── User not found ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsInvalidCredentialsException()
    {
        // Arrange
        _userRepo.GetByEmailAsync(Arg.Any<string>()).Returns((AppUser?)null);

        var command = new LoginCommand("nobody@test.com", "Password1!");

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    // ── Wrong password ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WrongPassword_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var user = MakeUser();
        _userRepo.GetByEmailAsync("priya@test.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "WrongPass").Returns(false);

        var command = new LoginCommand("priya@test.com", "WrongPass");

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    // ── Deactivated account ───────────────────────────────────────────────────

    [Fact]
    public async Task Handle_DeactivatedAccount_ThrowsAccountDeactivatedException()
    {
        // Arrange
        var user = MakeUser();
        user.Deactivate();

        _userRepo.GetByEmailAsync("priya@test.com").Returns(user);

        var command = new LoginCommand("priya@test.com", "Password1!");

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountDeactivatedException>();
    }
}
