// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using NoCap.Eats.Identity.Application.Commands.Register;

namespace NoCap.Eats.Identity.Tests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _sut = new();

    private static RegisterCommand Valid() => new(
        "priya Customer", "priya@test.com", "Password1!", "9934567890", "Customer");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _sut.Validate(Valid());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyFullName_HasError(string fullName)
    {
        var result = _sut.Validate(Valid() with { FullName = fullName });
        result.Errors.Should().Contain(e => e.PropertyName == "FullName");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Validate_InvalidEmail_HasError(string email)
    {
        var result = _sut.Validate(Valid() with { Email = email });
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("short")]      // too short
    [InlineData("alllower1")]  // no uppercase
    [InlineData("NOUPPER!")]   // no digit
    public void Validate_WeakPassword_HasError(string password)
    {
        var result = _sut.Validate(Valid() with { Password = password });
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("Password1")]   // 8 chars, upper + digit — valid
    [InlineData("SecurePass9")] // valid
    public void Validate_StrongPassword_HasNoPasswordError(string password)
    {
        var result = _sut.Validate(Valid() with { Password = password });
        result.Errors.Should().NotContain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("Hacker")]
    [InlineData("admin")]
    [InlineData("")]
    public void Validate_InvalidRole_HasError(string role)
    {
        var result = _sut.Validate(Valid() with { Role = role });
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [Theory]
    [InlineData("Customer")]
    [InlineData("RestaurantOwner")]
    [InlineData("DeliveryAgent")]
    [InlineData("Admin")]
    public void Validate_ValidRole_HasNoError(string role)
    {
        var result = _sut.Validate(Valid() with { Role = role });
        result.Errors.Should().NotContain(e => e.PropertyName == "Role");
    }

    [Theory]
    [InlineData("123")]           // too short
    [InlineData("notaphone")]     // letters
    public void Validate_InvalidMobileNumber_HasError(string phone)
    {
        var result = _sut.Validate(Valid() with { MobileNumber = phone });
        result.Errors.Should().Contain(e => e.PropertyName == "MobileNumber");
    }
}
