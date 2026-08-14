using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoCap.Eats.Identity.Application.Interfaces;
using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{
    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await userManager.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await userManager.Users.FirstOrDefaultAsync(
               u => u.NormalizedEmail == email.ToUpperInvariant(), ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(
               u => u.NormalizedEmail == email.ToUpperInvariant(), ct);
}
