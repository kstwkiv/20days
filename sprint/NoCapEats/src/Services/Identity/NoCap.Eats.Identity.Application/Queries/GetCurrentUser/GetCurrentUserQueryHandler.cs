// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.Application.Queries.GetCurrentUser;

/// <summary>Handles <see cref="GetCurrentUserQuery"/> by loading the user and their assigned role.</summary>
public class GetCurrentUserQueryHandler(
    IUserRepository      userRepository,
    UserManager<AppUser> userManager) : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    /// <summary>
    /// Retrieves the user record and resolves their role from ASP.NET Core Identity.
    /// </summary>
    /// <param name="request">Query containing the user ID to fetch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="UserDto"/> with the user's current profile and role.</returns>
    /// <exception cref="UserNotFoundException">Thrown when no user exists with the given ID.</exception>
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

        // Resolve the user's role for inclusion in the DTO
        var roles = await userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.MobileNumber,
            role,
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt);
    }
}
