using CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;
using FluentAssertions;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandValidatorTests
{
    private readonly UpdateDvdCommandValidator _validator = new();

    private static UpdateDvdCommand ValidCommand() =>
        new(Guid.NewGuid().ToString(), "Jaws 2", "Adventure", DateTime.Now.AddYears(-30), 3,
            Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));

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
    public void Validate_ShouldBeInvalid_WhenPublishedIsInTheFuture()
    {
        var command = ValidCommand() with { Published = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Published));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenGenreIsEmpty(string? genre)
    {
        var command = ValidCommand() with { Genre = genre! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.Genre));
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
    public void Validate_ShouldBeInvalid_WhenUpdatedAtIsInTheFuture()
    {
        var command = ValidCommand() with { UpdatedAt = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.UpdatedAt));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenDirectorIdIsEmpty(string? directorId)
    {
        var command = ValidCommand() with { DirectorId = directorId! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateDvdCommand.DirectorId));
    }
}
