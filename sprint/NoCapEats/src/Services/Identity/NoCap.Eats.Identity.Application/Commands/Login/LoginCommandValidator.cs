// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;

namespace NoCap.Eats.Identity.Application.Commands.Login;

/// <summary>FluentValidation rules for <see cref="LoginCommand"/>.</summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Configures basic presence and format rules for the login request.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
