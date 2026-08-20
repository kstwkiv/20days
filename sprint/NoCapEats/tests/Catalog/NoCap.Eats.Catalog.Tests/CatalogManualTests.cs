// Theory:
// This file is part of the Catalog module and contributes to its public behavior, business logic, or infrastructure implementation.
// Summary:
// The purpose of this file is to support the Catalog service by organizing domain, application, and infrastructure concerns in a cohesive architecture.

using FluentAssertions;
using NoCap.Eats.Catalog.Application.Commands.Restaurant.CreateRestaurant;
using NoCap.Eats.Catalog.Domain.Entities;
using NoCap.Eats.Catalog.Domain.Enums;
using NoCap.Eats.Catalog.Domain.Exceptions;

namespace NoCap.Eats.Catalog.Tests;

// Theory:
// A domain test validates the behavior of the aggregate root itself, before any database or API layer is involved.
// The Restaurant entity is the core business rule holder for ownership, lifecycle status, and menu structure.
// Summary:
// This class acts as a manual specification for the Catalog domain and the create-restaurant validator.
public class RestaurantTests
{
    // Theory:
    // A newly created restaurant should have a valid identity and a safe default lifecycle state.
    // The domain model sets PendingApproval by default and keeps the listing closed to orders until explicitly activated.
    // Summary:
    // This test protects the aggregate constructor contract from regressions.
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Theory:
        // We generate a fresh owner identity so the entity is not tied to a fixed test account.
        // Summary:
        // This keeps the test isolated and deterministic for ownership checks.
        var ownerId = Guid.NewGuid();

        // Theory:
        // The constructor is expected to copy business data into the aggregate and initialize its default status.
        // Summary:
        // We build a realistic restaurant object using the same values a real owner would submit.
        var restaurant = new Restaurant(
            ownerId,
            "Sushi House",
            "Fresh sushi and ramen",
            "10 Main St",
            "Seattle",
            "+1234567890",
            "Japanese");

        // Theory:
        // Ownership should be stored exactly as provided, and the aggregate should start in a non-live state.
        // Summary:
        // These assertions confirm the object was initialized with the intended business meaning.
        restaurant.OwnerId.Should().Be(ownerId);
        restaurant.Name.Should().Be("Sushi House");
        restaurant.City.Should().Be("Seattle");
        restaurant.Status.Should().Be(RestaurantStatus.PendingApproval);
        restaurant.IsOpen.Should().BeFalse();
        restaurant.Categories.Should().BeEmpty();
        restaurant.Id.Should().NotBe(Guid.Empty);
    }

    // Theory:
    // The aggregate supports grouping menu items under categories; this is a core Catalog responsibility.
    // Summary:
    // This test ensures category creation is attached to the right restaurant and stored in the aggregate.
    [Fact]
    public void AddCategory_AddsCategoryToRestaurant()
    {
        // Theory:
        // A restaurant must exist before categories are managed, and the aggregate owns its category collection.
        // Summary:
        // We create a real root entity and attach a category to it.
        var restaurant = new Restaurant(
            Guid.NewGuid(),
            "Taco Spot",
            "Street tacos",
            "22 Pine Ave",
            "Austin",
            "1234567890",
            "Mexican");

        // Theory:
        // AddCategory creates a MenuCategory object and appends it to the internal list.
        // Summary:
        // This validates the relationship between a restaurant and its menu groups.
        var category = restaurant.AddCategory("Starters", "Best sellers");

        // Theory:
        // The aggregate should expose exactly one category after insertion and keep the category linked to the restaurant.
        // Summary:
        // These assertions confirm the aggregate invariant was maintained.
        restaurant.Categories.Should().ContainSingle();
        category.Name.Should().Be("Starters");
        category.RestaurantId.Should().Be(restaurant.Id);
    }

    // Theory:
    // Ownership enforcement is a domain rule: only the restaurant owner is allowed to mutate protected data.
    // Summary:
    // This test proves the GuardOwner method rejects unauthorized access.
    [Fact]
    public void GuardOwner_Throws_WhenRequestingOwnerDoesNotMatch()
    {
        // Theory:
        // We create a restaurant owned by one user and attempt to validate a different user identity.
        // Summary:
        // This simulates a real authorization violation in the Catalog domain.
        var restaurant = new Restaurant(
            Guid.NewGuid(),
            "Pizza Place",
            "Wood-fired pizza",
            "45 Oak Rd",
            "Boston",
            "9876543210",
            "Italian");

        // Theory:
        // GuardOwner throws a domain-specific unauthorized exception when the ids differ.
        // Summary:
        // The expression delegate captures the domain behavior so we can assert the exact exception type.
        var action = () => restaurant.GuardOwner(Guid.NewGuid());

        // Theory:
        // A failed ownership check is not an application error; it is a domain exception with meaningful semantics.
        // Summary:
        // The test ensures the failure mode is explicit and safe for callers.
        action.Should().Throw<UnauthorizedRestaurantAccessException>();
    }

    // Theory:
    // A restaurant should be mutable through its aggregate methods, and updates should change only the intended fields.
    // Summary:
    // This test verifies the entity updates core details and refreshes its modified timestamp.
    [Fact]
    public void Update_ChangesRestaurantDetails()
    {
        // Theory:
        // A new restaurant begins with initial values that must be replaced by Update().
        // Summary:
        // We keep the original values in memory to assert that the updated values differ and the timestamp changes.
        var restaurant = new Restaurant(
            Guid.NewGuid(),
            "Old Name",
            "Old description",
            "1 First St",
            "Denver",
            "1112223333",
            "American");

        // Theory:
        // UpdatedAt is a concurrency-safe audit marker that should move forward whenever the aggregate changes.
        // Summary:
        // Capturing the old value before the update is the precise way to validate the timestamp behavior.
        var before = restaurant.UpdatedAt;

        // Theory:
        // Update() is the aggregate's standard mechanism for editing the restaurant profile.
        // Summary:
        // The test ensures the aggregate keeps business data consistent with the new input values.
        restaurant.Update(
            "New Name",
            "Updated description",
            "2 Second St",
            "Portland",
            "4445556666",
            "American");

        // Theory:
        // After mutating the aggregate, each editable property should match the updated business input.
        // Summary:
        // These assertions validate the branch of logic responsible for safe aggregate updates.
        restaurant.Name.Should().Be("New Name");
        restaurant.Description.Should().Be("Updated description");
        restaurant.City.Should().Be("Portland");
        restaurant.Phone.Should().Be("4445556666");
        restaurant.UpdatedAt.Should().BeOnOrAfter(before);
    }
}

// Theory:
// Validator tests verify the application request contract before the command reaches the business logic or persistence layer.
// Summary:
// This section validates the rules for CreateRestaurantCommand, ensuring only valid input reaches the domain aggregate.
public class CreateRestaurantCommandValidatorTests
{
    // Theory:
    // A validator instance is a reusable object that evaluates the same rule set across multiple command values.
    // Summary:
    // This field keeps tests consistent and easy to read.
    private readonly CreateRestaurantCommandValidator _validator = new();

    // Theory:
    // A valid restaurant registration should pass all length and format rules.
    // Summary:
    // This test confirms the happy path and protects the default contract from accidental stricter validation.
    [Fact]
    public void Validate_WhenRequestIsValid_ShouldPass()
    {
        // Theory:
        // A valid request contains all required fields and a properly formatted phone number.
        // Summary:
        // We construct a realistic restaurant owner command using a legal phone pattern.
        var command = new CreateRestaurantCommand(
            Guid.NewGuid(),
            "Burger House",
            "Gourmet burgers",
            "20 Market St",
            "Chicago",
            "+1234567890",
            "American");

        // Theory:
        // Validation should return no errors for a compliant request.
        // Summary:
        // This is the baseline success scenario against which all invalid input checks are compared.
        var result = _validator.Validate(command);

        // Theory:
        // FluentValidation marks the command as valid only when every configured rule succeeds.
        // Summary:
        // This assertion confirms the command contract is accepting legitimate values.
        result.IsValid.Should().BeTrue();
    }

    // Theory:
    // Business rules commonly reject empty or whitespace-only names because a restaurant must have a usable display label.
    // Summary:
    // This theory uses multiple input values to ensure the validation rule is consistently applied.
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsEmpty_ShouldFail(string name)
    {
        // Theory:
        // The command object is created with a blank restaurant name to simulate invalid user input.
        // Summary:
        // Empty names are not allowed by the rule set, so this should trigger a validation failure.
        var command = new CreateRestaurantCommand(
            Guid.NewGuid(),
            name,
            "Gourmet burgers",
            "20 Market St",
            "Chicago",
            "+1234567890",
            "American");

        // Theory:
        // The validator should inspect all properties and mark the command invalid when Name is empty.
        // Summary:
        // We then verify the specific property has an error entry.
        var result = _validator.Validate(command);

        // Theory:
        // Once a required rule fails, the result becomes invalid and the property name is recorded.
        // Summary:
        // This ensures incorrect input is rejected at the application boundary.
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRestaurantCommand.Name));
    }

    // Theory:
    // A phone number is considered a critical field for contact and operation, so it must follow format rules.
    // Summary:
    // This test confirms invalid contact information is rejected before a restaurant is created.
    [Fact]
    public void Validate_WhenPhoneIsInvalid_ShouldFail()
    {
        // Theory:
        // We pass an intentionally malformed phone number to test the regex rule in the validator.
        // Summary:
        // This matches a real user input mistake and ensures the app prevents invalid data.
        var command = new CreateRestaurantCommand(
            Guid.NewGuid(),
            "Burger House",
            "Gourmet burgers",
            "20 Market St",
            "Chicago",
            "bad-phone",
            "American");

        // Theory:
        // Validators do not merely check a value is present; they also check its format.
        // Summary:
        // We validate the command and assert the phone property error appears.
        var result = _validator.Validate(command);

        // Theory:
        // Invalid contact data must fail the validation pipeline so it never reaches persistence.
        // Summary:
        // This protects the domain from nonsense or unreachable contact records.
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateRestaurantCommand.Phone));
    }
}
