using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(
    Guid   UserId,
    string RawRefreshToken) : IRequest<AuthResponse>;
