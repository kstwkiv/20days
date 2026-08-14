using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
