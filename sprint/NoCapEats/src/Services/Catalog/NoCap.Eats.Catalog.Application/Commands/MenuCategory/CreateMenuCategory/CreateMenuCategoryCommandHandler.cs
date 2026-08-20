// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Application.Commands.MenuCategory.CreateMenuCategory;

/// <summary>
/// Handles <see cref="CreateMenuCategoryCommand"/> by verifying ownership
/// and adding a new category to the restaurant's aggregate.
/// </summary>
public class CreateMenuCategoryCommandHandler(
    IRestaurantRepository   restaurantRepo,
    IMenuCategoryRepository categoryRepo) : IRequestHandler<CreateMenuCategoryCommand, MenuCategoryDto>
{
    /// <summary>Adds a new menu category to the specified restaurant.</summary>
    /// <param name="request">Category details and the requesting owner's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="MenuCategoryDto"/> representing the new category.</returns>
    /// <exception cref="RestaurantNotFoundException">Thrown when the restaurant does not exist.</exception>
    /// <exception cref="UnauthorizedRestaurantAccessException">Thrown when the requester is not the owner.</exception>
    public async Task<MenuCategoryDto> Handle(CreateMenuCategoryCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await restaurantRepo.GetByIdAsync(request.RestaurantId, cancellationToken)
            ?? throw new RestaurantNotFoundException(request.RestaurantId);

        restaurant.GuardOwner(request.OwnerId);

        // AddCategory is an aggregate method that enforces invariants and adds to the internal list
        var category = restaurant.AddCategory(request.Name, request.Description);

        await restaurantRepo.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}
