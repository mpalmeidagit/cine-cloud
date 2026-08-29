using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Directors.Commands.DeleteDirector;

public class DeleteDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsWriteRepository> _repositoryMock = new();
    private readonly DeleteDirectorCommandHandler _handler;

    public DeleteDirectorCommandHandlerTests()
    {
        _handler = new DeleteDirectorCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenIdIsEmpty()
    {
        var command = new DeleteDirectorCommand(Guid.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.GetDirectorWithMovies(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDirectorDoesNotExist()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.GetDirectorWithMovies(command.Id)).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDirectorHasDvds()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid());
        var director = new Director("Steven", "Spielberg");
        var dvd = new Dvd("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());
        typeof(Director).GetField("_dvds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(director, new List<Dvd> { dvd });
        _repositoryMock.Setup(r => r.GetDirectorWithMovies(command.Id)).ReturnsAsync(director);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteAndReturnTrue_WhenDirectorExistsWithoutDvds()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid());
        var director = new Director("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.GetDirectorWithMovies(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Delete(command.Id)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.Delete(command.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryDeleteFails()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid());
        var director = new Director("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.GetDirectorWithMovies(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Delete(command.Id)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
