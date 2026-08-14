using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.BuildingBlocks.Events;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.Application.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser>   userManager,
    IUserRepository        userRepository,
    IPublishEndpoint       publisher) : IRequestHandler<RegisterCommand, UserDto>
{
    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new EmailAlreadyRegisteredException(request.Email);

        var user = new AppUser(request.FullName, request.Email, request.MobileNumber);

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new DomainException($"Registration failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, request.Role);

        await publisher.Publish(new UserRegisteredEvent(
            user.Id,
            user.FullName,
            user.Email!,
            request.Role,
            user.CreatedAt), cancellationToken);

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
