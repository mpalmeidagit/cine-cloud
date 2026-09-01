using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Commands.DeleteDirector;

public class DeleteDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsQueryRepository> _repositoryMock = new();
    private readonly DeleteDirectorCommandHandler _handler;

    public DeleteDirectorCommandHandlerTests()
    {
        _handler = new DeleteDirectorCommandHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldReturnFalse_WhenIdIsEmpty(string? id)
    {
        var command = new DeleteDirectorCommand(id!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDirectorDoesNotExist()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid().ToString());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Delete(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldDeleteAndReturnTrue_WhenDirectorExists()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid().ToString());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(new Director { Id = command.Id, FullName = "Steven Spielberg" });
        _repositoryMock.Setup(r => r.Delete(command.Id)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryDeleteFails()
    {
        var command = new DeleteDirectorCommand(Guid.NewGuid().ToString());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(new Director { Id = command.Id, FullName = "Steven Spielberg" });
        _repositoryMock.Setup(r => r.Delete(command.Id)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
