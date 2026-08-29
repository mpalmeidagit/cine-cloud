using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Dvds.Commands.CreateDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.CreateDvd;

public class CreateDvdCommandHandlerTests
{
    private readonly Mock<IDvdsWriteRepository> _repositoryMock = new();
    private readonly CreateDvdCommandHandler _handler;

    public CreateDvdCommandHandlerTests()
    {
        _handler = new CreateDvdCommandHandler(_repositoryMock.Object, new CreateDvdCommandValidator());
    }

    private static CreateDvdCommand ValidCommand() =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_ShouldReturnNullAndNotCreate_WhenCommandIsInvalid()
    {
        var command = ValidCommand() with { Title = "" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Create(It.IsAny<Dvd>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_WhenCommandIsValid()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Create(It.IsAny<Dvd>())).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Jaws");
        result.Genre.Should().Be("Action");
        result.Available.Should().BeTrue();
        result.Copies.Should().Be(5);
        result.DirectorId.Should().Be(command.DirectorId.ToString());
        _repositoryMock.Verify(r => r.Create(It.Is<Dvd>(d => d.Title == "Jaws")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryCreateFails()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Create(It.IsAny<Dvd>())).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
