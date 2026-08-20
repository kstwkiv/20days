// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.Login;

/// <summary>
/// Command to authenticate a user with email and password.
/// Returns JWT access token + refresh token on success.
/// </summary>
/// <param name="Email">Registered email address of the user.</param>
/// <param name="Password">Plain-text password to verify.</param>
public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
