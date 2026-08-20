// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Identity.Application.Interfaces;

namespace NoCap.Eats.Identity.Application.Commands.RevokeToken;

/// <summary>
/// Handles <see cref="RevokeTokenCommand"/> by marking all active refresh
/// tokens for the user as revoked, effectively logging them out everywhere.
/// </summary>
public class RevokeTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepo) : IRequestHandler<RevokeTokenCommand>
{
    /// <summary>Revokes all active refresh tokens for the specified user.</summary>
    /// <param name="request">Contains the user ID to logout.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        // Mark every non-revoked token for this user as revoked
        await refreshTokenRepo.RevokeAllForUserAsync(request.UserId, cancellationToken);
        await refreshTokenRepo.SaveChangesAsync(cancellationToken);
    }
}
