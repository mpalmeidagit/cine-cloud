using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.CreateDvd;

public class CreateDvdCommandHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly CreateDvdCommandHandler _handler;

    public CreateDvdCommandHandlerTests()
    {
        _handler = new CreateDvdCommandHandler(_repositoryMock.Object, new CreateDvdCommandValidator());
    }

    private static CreateDvdCommand ValidCommand(string? id = null) =>
        new(id ?? Guid.NewGuid().ToString(), "Jaws", "Action", DateTime.Now.AddYears(-40), true, 5,
            Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(-1));

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
    {
        var command = ValidCommand() with { Title = "" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDvdAlreadyExists()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(new Dvd { Id = command.Id, Title = "Existing" });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Create(It.IsAny<Dvd>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryCreateReturnsNull()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Dvd>())).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenDvdIsCreated()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Dvd>())).ReturnsAsync(new Dvd { Id = command.Id, Title = command.Title });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.Create(It.Is<Dvd>(d => d.Id == command.Id && d.Title == command.Title)), Times.Once);
    }
}
