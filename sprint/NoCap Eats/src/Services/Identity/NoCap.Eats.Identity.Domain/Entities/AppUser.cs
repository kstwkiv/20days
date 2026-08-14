using Microsoft.AspNetCore.Identity;

namespace NoCap.Eats.Identity.Domain.Entities;

/// <summary>
/// Application user — extends ASP.NET Core Identity with NoCap Eats-specific fields.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; private set; } = default!;
    public string MobileNumber { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }

    // EF Core requires a parameterless constructor
    protected AppUser() { }

    public AppUser(string fullName, string email, string mobileNumber)
    {
        Id           = Guid.NewGuid();
        FullName     = fullName;
        Email        = email;
        UserName     = email;
        MobileNumber = mobileNumber;
        CreatedAt    = DateTime.UtcNow;
    }

    public void UpdateProfile(string fullName, string mobileNumber)
    {
        FullName     = fullName;
        MobileNumber = mobileNumber;
    }

    public void RecordLogin()  => LastLoginAt = DateTime.UtcNow;
    public void Deactivate()   => IsActive = false;
    public void Reactivate()   => IsActive = true;
}
