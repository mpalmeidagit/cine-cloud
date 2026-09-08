using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Dvds.Commands.CreateDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.CreateDvd;

public class CreateDvdCommandValidatorTests
{
    private readonly Mock<IDirectorsWriteRepository> _directorsRepositoryMock = new();
    private readonly CreateDvdCommandValidator _validator;

    public CreateDvdCommandValidatorTests()
    {
        _directorsRepositoryMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(new Director("Steven", "Spielberg"));
        _validator = new CreateDvdCommandValidator(_directorsRepositoryMock.Object);
    }

    private static CreateDvdCommand ValidCommand() =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

    [Fact]
    public async Task Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        var result = await _validator.ValidateAsync(ValidCommand());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenDirectorDoesNotExist()
    {
        var command = ValidCommand();
        _directorsRepositoryMock.Setup(r => r.Get(command.DirectorId)).ReturnsAsync((Director)null!);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.DirectorId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_ShouldBeInvalid_WhenTitleIsEmpty(string? title)
    {
        var command = ValidCommand() with { Title = title! };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenTitleIsShorterThanMinLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MIN_TITLE_LENGTH - 1) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenTitleIsLongerThanMaxLength()
    {
        var command = ValidCommand() with { Title = new string('a', Dvd.MAX_TITLE_LENGTH + 1) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Title));
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenGenreIsGreaterOrEqualTo19()
    {
        var command = ValidCommand() with { Genre = 19 };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Genre));
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenGenreIsNegative()
    {
        var command = ValidCommand() with { Genre = -1 };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Genre));
    }

    [Fact]
    public async Task Validate_ShouldBeValid_WhenGenreIsTheHighestValidValue()
    {
        var command = ValidCommand() with { Genre = 18 };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenPublishedDateIsInTheFuture()
    {
        var command = ValidCommand() with { Published = DateTime.Now.AddDays(1) };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Published));
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenCopiesIsNegative()
    {
        var command = ValidCommand() with { Copies = -1 };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.Copies));
    }

    [Fact]
    public async Task Validate_ShouldBeValid_WhenCopiesIsZero()
    {
        var command = ValidCommand() with { Copies = 0 };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldBeInvalid_WhenDirectorIdIsEmpty()
    {
        var command = ValidCommand() with { DirectorId = Guid.Empty };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateDvdCommand.DirectorId));
    }
}
