using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CineCloud.Application.Tests.Features.Directors.Commands.CreateDirector;

public class CreateDirectorCommandValidatorTests
{
    private readonly CreateDirectorCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldBeValid_WhenNameAndSurnameAreValid()
    {
        var command = new CreateDirectorCommand("Steven", "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenNameIsEmpty(string? name)
    {
        var command = new CreateDirectorCommand(name!, "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Name));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenNameIsShorterThanMinLength()
    {
        var command = new CreateDirectorCommand(new string('a', Director.MIN_LENGTH - 1), "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Name));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenNameIsLongerThanMaxLength()
    {
        var command = new CreateDirectorCommand(new string('a', Director.MAX_LENGTH + 1), "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenSurnameIsEmpty(string? surname)
    {
        var command = new CreateDirectorCommand("Steven", surname!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Surname));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenSurnameIsShorterThanMinLength()
    {
        var command = new CreateDirectorCommand("Steven", new string('a', Director.MIN_LENGTH - 1));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Surname));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenSurnameIsLongerThanMaxLength()
    {
        var command = new CreateDirectorCommand("Steven", new string('a', Director.MAX_LENGTH + 1));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDirectorCommand.Surname));
    }
}
