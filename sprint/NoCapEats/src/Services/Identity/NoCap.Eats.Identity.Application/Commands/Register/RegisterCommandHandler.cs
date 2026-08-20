// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.BuildingBlocks.Events;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.Application.Commands.Register;

/// <summary>
/// Handles the <see cref="RegisterCommand"/> by creating a new user,
/// assigning the requested role, and publishing a <see cref="UserRegisteredEvent"/>.
/// and
/// </summary>
public class RegisterCommandHandler(
    UserManager<AppUser>   userManager,
    IUserRepository        userRepository,
    IPublishEndpoint       publisher) : IRequestHandler<RegisterCommand, UserDto>
{
    /// <summary>
    /// Executes the registration flow: validates uniqueness, creates the user,
    /// assigns the role, then broadcasts the integration event.
    /// </summary>
    /// <param name="request">Registration details supplied by the caller.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="UserDto"/> representing the newly created user.</returns>
    /// <exception cref="EmailAlreadyRegisteredException">Thrown if the email is already in use.</exception>
    /// <exception cref="DomainException">Thrown if ASP.NET Core Identity reports creation errors.</exception>
    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Guard against duplicate email addresses before attempting creation
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new EmailAlreadyRegisteredException(request.Email);

        var user = new AppUser(request.FullName, request.Email, request.MobileNumber);

        // Delegate password hashing and storage to ASP.NET Core Identity
        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new DomainException($"Registration failed: {errors}");
        }

        // Assign the requested role so downstream JWT claims are correct
        await userManager.AddToRoleAsync(user, request.Role);

        // Fire-and-forget — don't block the response if RabbitMQ is unavailable
        _ = publisher.Publish(new UserRegisteredEvent(
            user.Id,
            user.FullName,
            user.Email!,
            request.Role,
            user.CreatedAt), cancellationToken)
            .ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnFaulted);

        return new UserDto(
            user.Id,
            user.FullName,
            user.Email!,
            user.MobileNumber,
            request.Role,
            user.IsActive,
            user.CreatedAt,
            user.LastLoginAt);
    }
}
