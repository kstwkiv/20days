namespace NoCap.Eats.Identity.Domain.Enums;

public static class UserRole
{
    public const string Customer          = "Customer";
    public const string RestaurantOwner   = "RestaurantOwner";
    public const string DeliveryAgent     = "DeliveryAgent";
    public const string Admin             = "Admin";

    public static readonly IReadOnlyList<string> All =
    [
        Customer, RestaurantOwner, DeliveryAgent, Admin
    ];
}
