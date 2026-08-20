// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Application.Commands.MenuItem.CreateMenuItem;

/// <summary>
/// Handles <see cref="CreateMenuItemCommand"/> by resolving the parent category,
/// verifying ownership through the restaurant, and adding the item.
/// </summary>
public class CreateMenuItemCommandHandler(
    IMenuCategoryRepository categoryRepo,
    IRestaurantRepository   restaurantRepo) : IRequestHandler<CreateMenuItemCommand, MenuItemDto>
{
    /// <summary>Adds a new menu item to the specified category after verifying ownership.</summary>
    /// <param name="request">Item details and the requesting owner's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="MenuItemDto"/> representing the new item.</returns>
    /// <exception cref="MenuCategoryNotFoundException">Thrown when the category does not exist.</exception>
    /// <exception cref="RestaurantNotFoundException">Thrown when the parent restaurant does not exist.</exception>
    /// <exception cref="UnauthorizedRestaurantAccessException">Thrown when the requester is not the owner.</exception>
    public async Task<MenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new MenuCategoryNotFoundException(request.CategoryId);

        // Walk up to the restaurant to verify ownership — categories don't store OwnerId directly
        var restaurant = await restaurantRepo.GetByIdAsync(category.RestaurantId, cancellationToken)
            ?? throw new RestaurantNotFoundException(category.RestaurantId);

        restaurant.GuardOwner(request.OwnerId);

        var item = category.AddItem(request.Name, request.Description, request.Price);

        await categoryRepo.SaveChangesAsync(cancellationToken);

        return item.ToDto();
    }
}
