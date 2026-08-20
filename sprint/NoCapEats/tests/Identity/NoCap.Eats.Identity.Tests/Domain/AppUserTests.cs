// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using NoCap.Eats.Identity.Domain.Entities;

namespace NoCap.Eats.Identity.Tests.Domain;

public class AppUserTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var user = new AppUser("priya customer", "priya@test.com", "9934567890");

        user.FullName.Should().Be("priya customer");
        user.Email.Should().Be("priya@test.com");
        user.UserName.Should().Be("priya@test.com");
        user.MobileNumber.Should().Be("9934567890");
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var user = new AppUser("priya", "p@t.com", "+1");
        user.Deactivate();
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Reactivate_SetsIsActiveToTrue()
    {
        var user = new AppUser("priya", "p@t.com", "+1");
        user.Deactivate();
        user.Reactivate();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RecordLogin_SetsLastLoginAt()
    {
        var user = new AppUser("priya", "p@t.com", "+1");
        user.LastLoginAt.Should().BeNull();

        var before = DateTime.UtcNow;
        user.RecordLogin();

        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void UpdateProfile_ChangesFullNameAndMobile()
    {
        var user = new AppUser("priya", "p@t.com", "+1");
        user.UpdateProfile("priya Updated", "+9999999999");

        user.FullName.Should().Be("priya Updated");
        user.MobileNumber.Should().Be("+9999999999");
    }
}
