using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string MobileNumber,
    string Role) : IRequest<UserDto>;
