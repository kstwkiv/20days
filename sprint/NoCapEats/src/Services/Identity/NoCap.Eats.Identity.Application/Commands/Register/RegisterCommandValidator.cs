// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;
using NoCap.Eats.Identity.Domain.Enums;

namespace NoCap.Eats.Identity.Application.Commands.Register;

/// <summary>
/// FluentValidation rules for <see cref="RegisterCommand"/>.
/// Enforces password complexity, email format, phone format, and valid role values.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    /// <summary>Configures all validation rules for the registration request.</summary>
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        // Password must meet minimum strength: 8 chars, 1 uppercase, 1 digit
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        // E.164-compatible international phone number
        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Invalid mobile number format.");

        // Role must exist in the known set to prevent privilege escalation
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => UserRole.All.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", UserRole.All)}.");
    }
}
