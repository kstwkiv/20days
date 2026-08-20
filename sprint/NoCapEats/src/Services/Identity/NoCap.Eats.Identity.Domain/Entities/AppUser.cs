// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using Microsoft.AspNetCore.Identity;

namespace NoCap.Eats.Identity.Domain.Entities;

/// <summary>
/// Application user — extends ASP.NET Core Identity with NoCap Eats-specific fields.
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    /// <summary>Full display name of the user.</summary>
    public string FullName { get; private set; } = default!;

    /// <summary>Mobile phone number of the user.</summary>
    public string MobileNumber { get; private set; } = default!;

    /// <summary>Indicates whether the account is currently active.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>UTC timestamp when the account was created.</summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the user's most recent successful login, or <c>null</c> if never logged in.</summary>
    public DateTime? LastLoginAt { get; private set; }

    // EF Core requires a parameterless constructor
    /// <summary>Parameterless constructor required by EF Core.</summary>
    protected AppUser() { }

    /// <summary>Creates a new <see cref="AppUser"/> with the provided identity details.</summary>
    /// <param name="fullName">Full display name of the user.</param>
    /// <param name="email">Email address used as the login identifier.</param>
    /// <param name="mobileNumber">Mobile phone number of the user.</param>
    public AppUser(string fullName, string email, string mobileNumber)
    {
        Id           = Guid.NewGuid();
        FullName     = fullName;
        Email        = email;
        UserName     = email;
        MobileNumber = mobileNumber;
        CreatedAt    = DateTime.UtcNow;
    }

    /// <summary>Updates the user's full name and mobile number.</summary>
    /// <param name="fullName">New full display name.</param>
    /// <param name="mobileNumber">New mobile phone number.</param>
    public void UpdateProfile(string fullName, string mobileNumber)
    {
        FullName     = fullName;
        MobileNumber = mobileNumber;
    }

    /// <summary>Records the current UTC time as the user's last successful login.</summary>
    public void RecordLogin()  => LastLoginAt = DateTime.UtcNow;

    /// <summary>Deactivates the user account.</summary>
    public void Deactivate()   => IsActive = false;

    /// <summary>Reactivates a previously deactivated user account.</summary>
    public void Reactivate()   => IsActive = true;
}
