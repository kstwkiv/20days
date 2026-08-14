namespace NoCap.Eats.Identity.Application.DTOs;

public record UserDto(
    Guid   Id,
    string FullName,
    string Email,
    string MobileNumber,
    string Role,
    bool   IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt);
