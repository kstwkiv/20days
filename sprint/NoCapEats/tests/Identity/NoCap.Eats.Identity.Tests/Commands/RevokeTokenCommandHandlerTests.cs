// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using NoCap.Eats.Identity.Application.Commands.RevokeToken;
using NoCap.Eats.Identity.Application.Interfaces;
using NSubstitute;

namespace NoCap.Eats.Identity.Tests.Commands;

public class RevokeTokenCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly RevokeTokenCommandHandler _sut;

    public RevokeTokenCommandHandlerTests()
    {
        _refreshRepo = Substitute.For<IRefreshTokenRepository>();
        _sut         = new RevokeTokenCommandHandler(_refreshRepo);
    }

    [Fact]
    public async Task Handle_ValidUserId_RevokesAllTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RevokeTokenCommand(userId);

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert — both methods called exactly once
        await _refreshRepo.Received(1).RevokeAllForUserAsync(userId, Arg.Any<CancellationToken>());
        await _refreshRepo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
