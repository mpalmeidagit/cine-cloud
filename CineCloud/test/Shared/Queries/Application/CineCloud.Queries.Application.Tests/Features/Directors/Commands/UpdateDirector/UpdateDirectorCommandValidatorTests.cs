using CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;
using FluentAssertions;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandValidatorTests
{
    private readonly UpdateDirectorCommandValidator _validator = new();

    private static UpdateDirectorCommand ValidCommand() =>
        new(Guid.NewGuid().ToString(), "George Lucas", DateTime.Now.AddMinutes(-1));

    [Fact]
    public void Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenIdIsEmpty(string? id)
    {
        var command = ValidCommand() with { Id = id! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenFullNameIsEmpty(string? fullName)
    {
        var command = ValidCommand() with { FullName = fullName! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.FullName));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenFullNameIsShorterThanMinLength()
    {
        var command = ValidCommand() with { FullName = "ab" };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.FullName));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenFullNameIsLongerThanMaxLength()
    {
        var command = ValidCommand() with { FullName = new string('a', 61) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.FullName));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenUpdatedAtIsInTheFuture()
    {
        var command = ValidCommand() with { UpdatedAt = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.UpdatedAt));
    }
}
