// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Identity.Application.DTOs;

namespace NoCap.Eats.Identity.Application.Commands.Register;

/// <summary>
/// Command to register a new user account.
/// Handled by <see cref="RegisterCommandHandler"/>.
/// </summary>
/// <param name="FullName">Full display name of the new user.</param>
/// <param name="Email">Email address used as the login identifier.</param>
/// <param name="Password">Plain-text password — hashed by ASP.NET Core Identity.</param>
/// <param name="MobileNumber">Contact mobile number in E.164 format.</param>
/// <param name="Role">Role to assign — must be one of the values in <see cref="Domain.Enums.UserRole"/>.</param>
public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string MobileNumber,
    string Role) : IRequest<UserDto>;
