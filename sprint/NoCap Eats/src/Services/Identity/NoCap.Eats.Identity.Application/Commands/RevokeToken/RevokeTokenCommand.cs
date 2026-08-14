using MediatR;

namespace NoCap.Eats.Identity.Application.Commands.RevokeToken;

/// <summary>Revokes all refresh tokens for the given user (logout).</summary>
public record RevokeTokenCommand(Guid UserId) : IRequest;
