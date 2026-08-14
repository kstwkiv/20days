using FluentValidation;
using NoCap.Eats.Identity.Domain.Enums;

namespace NoCap.Eats.Identity.Application.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.MobileNumber)
            .NotEmpty().WithMessage("Mobile number is required.")
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Invalid mobile number format.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => UserRole.All.Contains(r))
            .WithMessage($"Role must be one of: {string.Join(", ", UserRole.All)}.");
    }
}
