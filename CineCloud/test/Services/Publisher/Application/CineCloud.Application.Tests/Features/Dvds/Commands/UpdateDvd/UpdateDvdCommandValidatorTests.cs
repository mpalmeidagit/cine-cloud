using CineCloud.Application.Features.Dvds.Commands.UpdateDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandValidatorTests
{
    private readonly UpdateDvdCommandValidator _validator = new();

    private static UpdateDvdCommand ValidCommand() =>
        new(Guid.NewGuid(), "Jaws", 0, DateTime.Now.AddYears(-40), Guid.NewGuid(), 5);

    [Fact]
    public void Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        var result = _validator.Validate(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenIdIsEmpty()
    {
        var command = ValidCommand() with { Id = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenTitleIsEmpty(string? title)
    {
        var command = ValidCommand() with { Title = title! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenTitleIsShorterThanMinLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MIN_TITLE_LENGTH - 1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenTitleIsLongerThanMaxLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MAX_TITLE_LENGTH + 1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Title));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenGenreIsGreaterOrEqualTo19()
    {
        var command = ValidCommand() with { Genre = 19 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Genre));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenGenreIsNegative()
    {
        var command = ValidCommand() with { Genre = -1 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Genre));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenPublishedDateIsInTheFuture()
    {
        var command = ValidCommand() with { Published = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Published));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenCopiesIsNegative()
    {
        var command = ValidCommand() with { Copies = -1 };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Copies));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenDirectorIdIsEmpty()
    {
        var command = ValidCommand() with { DirectorId = Guid.Empty };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.DirectorId));
    }
}
