// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using MediatR;
using NoCap.Eats.Delivery.Application.DTOs;
using NoCap.Eats.Delivery.Domain.Enums;

namespace NoCap.Eats.Delivery.Application.Commands.UpdateDeliveryStatus;

public record UpdateDeliveryStatusCommand(
    Guid           DeliveryId,
    Guid           AgentId,
    DeliveryStatus TargetStatus) : IRequest<DeliveryDto>;
