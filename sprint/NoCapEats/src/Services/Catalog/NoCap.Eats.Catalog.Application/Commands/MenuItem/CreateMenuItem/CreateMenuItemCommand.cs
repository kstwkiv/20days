// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Catalog.Application.DTOs;

namespace NoCap.Eats.Catalog.Application.Commands.MenuItem.CreateMenuItem;

public record CreateMenuItemCommand(
    Guid    CategoryId,
    Guid    OwnerId,
    string  Name,
    string  Description,
    decimal Price) : IRequest<MenuItemDto>;
