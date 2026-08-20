// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;

namespace NoCap.Eats.Identity.Application.Commands.RevokeToken;

/// <summary>
/// Command to log out a user by revoking all their active refresh tokens.
/// Prevents any further token rotation until the user logs in again.
/// </summary>
/// <param name="UserId">ID of the user whose tokens should be revoked.</param>
public record RevokeTokenCommand(Guid UserId) : IRequest;
