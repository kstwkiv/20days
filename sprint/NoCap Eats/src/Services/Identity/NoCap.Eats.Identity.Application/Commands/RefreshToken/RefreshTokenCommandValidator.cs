using FluentValidation;

namespace NoCap.Eats.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.RawRefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
