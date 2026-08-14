using MediatR;
using Microsoft.AspNetCore.Identity;
using NoCap.Eats.Identity.Application.DTOs;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;
using NoCap.Eats.Identity.Domain.Exceptions;

namespace NoCap.Eats.Identity.Application.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IUserRepository    userRepository,
    UserManager<AppUser> userManager) : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new UserNotFoundException(request.UserId);

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
