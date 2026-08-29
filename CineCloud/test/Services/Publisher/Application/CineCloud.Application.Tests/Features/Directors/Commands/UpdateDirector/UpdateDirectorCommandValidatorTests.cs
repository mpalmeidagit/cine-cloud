using CineCloud.Application.Features.Directors.Commands.UpdateDirector;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CineCloud.Application.Tests.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandValidatorTests
{
    private readonly UpdateDirectorCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "Steven", "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenIdIsEmpty()
    {
        var command = new UpdateDirectorCommand(Guid.Empty, "Steven", "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenNameIsEmpty(string? name)
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), name!, "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Name));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenNameIsShorterThanMinLength()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), new string('a', Director.MIN_LENGTH - 1), "Spielberg");

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Name));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenSurnameIsEmpty(string? surname)
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "Steven", surname!);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Surname));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenSurnameIsShorterThanMinLength()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "Steven", new string('a', Director.MIN_LENGTH - 1));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDirectorCommand.Surname));
    }
}
