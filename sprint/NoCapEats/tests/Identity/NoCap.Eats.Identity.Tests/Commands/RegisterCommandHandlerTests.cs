// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.Commands.Register;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;
using NSubstitute;

namespace NoCap.Eats.Identity.Tests.Commands;

public class RegisterCommandHandlerTests
{
    private readonly UserManager<AppUser>  _userManager;
    private readonly IUserRepository       _userRepo;
    private readonly IPublishEndpoint      _publisher;
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
    {
        // UserManager requires a store — use a substitute of the abstract base
        var store = Substitute.For<IUserStore<AppUser>>();
        _userManager = Substitute.For<UserManager<AppUser>>(
            store, null, null, null, null, null, null, null, null);

        _userRepo  = Substitute.For<IUserRepository>();
        _publisher = Substitute.For<IPublishEndpoint>();
        _sut       = new RegisterCommandHandler(_userManager, _userRepo, _publisher);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUserDto()
    {
        // Arrange
        var command = new RegisterCommand(
            "priya Customer", "priya@test.com", "Password1!", "9934567890", "Customer");

        _userRepo.EmailExistsAsync("priya@test.com").Returns(false);
        _userManager.CreateAsync(Arg.Any<AppUser>(), "Password1!")
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), "Customer")
            .Returns(IdentityResult.Success);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("priya@test.com");
        result.FullName.Should().Be("priya Customer");
        result.Role.Should().Be("Customer");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_PublishesUserRegisteredEvent()
    {
        // Arrange
        var command = new RegisterCommand(
            "Bob Owner", "bob@test.com", "Password1!", "9234567890", "RestaurantOwner");

        _userRepo.EmailExistsAsync("bob@test.com").Returns(false);
        _userManager.CreateAsync(Arg.Any<AppUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);
        _userManager.AddToRoleAsync(Arg.Any<AppUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Success);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert — publisher must have been called once
        await _publisher.Received(1).Publish(
            Arg.Any<NoCap.Eats.BuildingBlocks.Events.UserRegisteredEvent>(),
            Arg.Any<CancellationToken>());
    }

    // ── Duplicate email ───────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsEmailAlreadyRegisteredException()
    {
        // Arrange
        var command = new RegisterCommand(
            "Dup User", "dup@test.com", "Password1!", "9934567890", "Customer");

        _userRepo.EmailExistsAsync("dup@test.com").Returns(true);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EmailAlreadyRegisteredException>()
            .WithMessage("*dup@test.com*");
    }

    // ── Identity creation failure ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_IdentityCreateFails_ThrowsDomainException()
    {
        // Arrange
        var command = new RegisterCommand(
            "Bad User", "bad@test.com", "weak", "9934567890", "Customer");

        _userRepo.EmailExistsAsync("bad@test.com").Returns(false);
        _userManager.CreateAsync(Arg.Any<AppUser>(), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError
                { Description = "Password too weak." }));

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Registration failed*");
    }
}
