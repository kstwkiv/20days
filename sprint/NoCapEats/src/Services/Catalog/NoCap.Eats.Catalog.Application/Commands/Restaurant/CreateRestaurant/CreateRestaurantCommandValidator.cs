// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentValidation;

namespace NoCap.Eats.Catalog.Application.Commands.Restaurant.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(150);

        RuleFor(x => x.Description)
            .NotEmpty().MaximumLength(1000);

        RuleFor(x => x.Address)
            .NotEmpty().MaximumLength(300);

        RuleFor(x => x.City)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Invalid phone number.");
    }
}
