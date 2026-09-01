using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Commands.ReturnDvd;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.ReturnDvd;

public class ReturnDvdCommandHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly ReturnDvdCommandHandler _handler;

    public ReturnDvdCommandHandlerTests()
    {
        _handler = new ReturnDvdCommandHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldReturnFalse_WhenIdIsEmpty(string? id)
    {
        var command = new ReturnDvdCommand(id!, DateTime.Now.AddMinutes(-1));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenUpdatedAtIsInTheFuture()
    {
        var command = new ReturnDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddDays(1));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDvdDoesNotExist()
    {
        var command = new ReturnDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldIncrementCopiesAndReturnTrue_WhenDvdExists()
    {
        var command = new ReturnDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        var dvd = new Dvd { Id = command.Id, Copies = 3 };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        dvd.Copies.Should().Be(4);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryUpdateFails()
    {
        var command = new ReturnDvdCommand(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        var dvd = new Dvd { Id = command.Id, Copies = 3 };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
