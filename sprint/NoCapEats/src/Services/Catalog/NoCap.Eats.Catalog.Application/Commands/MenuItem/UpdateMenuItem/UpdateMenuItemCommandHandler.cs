// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;
using NoCap.Eats.Catalog.Application.Interfaces;
using NoCap.Eats.Catalog.Application.Mappings;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Application.Commands.MenuItem.UpdateMenuItem;

/// <summary>
/// Handles <see cref="UpdateMenuItemCommand"/> by resolving the ownership chain
/// (item → category → restaurant) and applying the updated values.
/// </summary>
public class UpdateMenuItemCommandHandler(
    IMenuItemRepository     itemRepo,
    IMenuCategoryRepository categoryRepo,
    IRestaurantRepository   restaurantRepo) : IRequestHandler<UpdateMenuItemCommand, MenuItemDto>
{
    /// <summary>Updates a menu item's details after verifying the requesting user owns the restaurant.</summary>
    /// <param name="request">Update details including item ID and new values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated <see cref="MenuItemDto"/>.</returns>
    /// <exception cref="MenuItemNotFoundException">Thrown when the item does not exist.</exception>
    /// <exception cref="MenuCategoryNotFoundException">Thrown when the parent category does not exist.</exception>
    /// <exception cref="RestaurantNotFoundException">Thrown when the parent restaurant does not exist.</exception>
    /// <exception cref="UnauthorizedRestaurantAccessException">Thrown when the requester is not the restaurant owner.</exception>
    public async Task<MenuItemDto> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepo.GetByIdAsync(request.MenuItemId, cancellationToken)
            ?? throw new MenuItemNotFoundException(request.MenuItemId);

        var category = await categoryRepo.GetByIdAsync(item.CategoryId, cancellationToken)
            ?? throw new MenuCategoryNotFoundException(item.CategoryId);

        var restaurant = await restaurantRepo.GetByIdAsync(category.RestaurantId, cancellationToken)
            ?? throw new RestaurantNotFoundException(category.RestaurantId);

        restaurant.GuardOwner(request.OwnerId);

        item.Update(request.Name, request.Description, request.Price, request.ImageUrl);

        await itemRepo.SaveChangesAsync(cancellationToken);
        return item.ToDto();
    }
}
