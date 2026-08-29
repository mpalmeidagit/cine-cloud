using CineCloud.Application.Features.Dvds.Commands.CreateDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.CreateDvd;

public class CreateDvdCommandValidatorTests
{
    private readonly CreateDvdCommandValidator _validator = new();

    private static CreateDvdCommand ValidCommand() =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

    [Fact]
    public void Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenTitleIsEmpty(string? title)
    {
        var command = ValidCommand() with { Title = title! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenTitleIsShorterThanMinLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MIN_TITLE_LENGTH - 1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenTitleIsLongerThanMaxLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MAX_TITLE_LENGTH + 1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenGenreIsGreaterOrEqualTo19()
    {
        var command = ValidCommand() with { Genre = 19 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Genre));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenGenreIsNegative()
    {
        var command = ValidCommand() with { Genre = -1 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Genre));
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenGenreIsTheHighestValidValue()
    {
        var command = ValidCommand() with { Genre = 18 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenPublishedDateIsInTheFuture()
    {
        var command = ValidCommand() with { Published = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Published));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenCopiesIsNegative()
    {
        var command = ValidCommand() with { Copies = -1 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Copies));
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenCopiesIsZero()
    {
        var command = ValidCommand() with { Copies = 0 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenDirectorIdIsEmpty()
    {
        var command = ValidCommand() with { DirectorId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.DirectorId));
    }
}
