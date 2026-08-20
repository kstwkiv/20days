// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

/// <summary>FluentValidation rules for <see cref="RefreshTokenCommand"/>.</summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>Ensures both required fields are present before the handler is invoked.</summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.RawRefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
