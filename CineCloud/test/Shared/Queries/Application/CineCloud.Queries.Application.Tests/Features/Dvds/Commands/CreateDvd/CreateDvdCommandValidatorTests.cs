using CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;
using FluentAssertions;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.CreateDvd;

public class CreateDvdCommandValidatorTests
{
    private readonly CreateDvdCommandValidator _validator = new();

    private static CreateDvdCommand ValidCommand() =>
        new(Guid.NewGuid().ToString(), "Jaws", "Action", DateTime.Now.AddYears(-40), true, 5,
            Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(-1));

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
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Id));
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
    public void Validate_ShouldBeInvalid_WhenPublishedIsInTheFuture()
    {
        var command = ValidCommand() with { Published = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Published));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenGenreIsEmpty(string? genre)
    {
        var command = ValidCommand() with { Genre = genre! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Genre));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenAvailableIsFalse()
    {
        var command = ValidCommand() with { Available = false };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Available));
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
    public void Validate_ShouldBeInvalid_WhenCreatedAtIsInTheFuture()
    {
        var command = ValidCommand() with { CreatedAt = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.CreatedAt));
    }

    [Fact]
    public void Validate_ShouldBeInvalid_WhenUpdatedAtIsInTheFuture()
    {
        var command = ValidCommand() with { UpdatedAt = DateTime.Now.AddDays(1) };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.UpdatedAt));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldBeInvalid_WhenDirectorIdIsEmpty(string? directorId)
    {
        var command = ValidCommand() with { DirectorId = directorId! };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.DirectorId));
    }
}
