// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

/// <summary>
/// Command to exchange a valid raw refresh token for a new access token and rotated refresh token.
/// </summary>
/// <param name="UserId">ID of the user requesting token rotation.</param>
/// <param name="RawRefreshToken">The plain-text refresh token previously issued to the client.</param>
public record RefreshTokenCommand(
    Guid   UserId,
    string RawRefreshToken) : IRequest<AuthResponse>;
