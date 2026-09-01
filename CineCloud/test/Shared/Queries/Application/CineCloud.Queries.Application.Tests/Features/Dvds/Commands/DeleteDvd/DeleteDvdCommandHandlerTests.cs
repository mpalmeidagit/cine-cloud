using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Commands.DeleteDvd;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.DeleteDvd;

public class DeleteDvdCommandHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly DeleteDvdCommandHandler _handler;

    public DeleteDvdCommandHandlerTests()
    {
        _handler = new DeleteDvdCommandHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldReturnFalse_WhenIdIsEmpty(string? id)
    {
        var command = new DeleteDvdCommand(id!, DateTime.Now.AddMinutes(-1));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDeletedAtIsInTheFuture()
    {
        var command = new DeleteDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddDays(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDvdDoesNotExist()
    {
        var command = new DeleteDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldMarkDvdAsUnavailableAndReturnTrue_WhenDvdExists()
    {
        var command = new DeleteDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        var dvd = new Dvd { Id = command.Id, Available = true, Copies = 3 };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        dvd.Available.Should().BeFalse();
        dvd.Copies.Should().Be(0);
        dvd.DeletedAt.Should().Be(command.DeletedAt);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryUpdateFails()
    {
        var command = new DeleteDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        var dvd = new Dvd { Id = command.Id, Available = true, Copies = 3 };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
