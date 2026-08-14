using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Application.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool>     EmailExistsAsync(string email, CancellationToken ct = default);
}
