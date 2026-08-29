using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Dvds.Commands.DeleteDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.DeleteDvd;

public class DeleteDvdCommandHandlerTests
{
    private readonly Mock<IDvdsWriteRepository> _repositoryMock = new();
    private readonly DeleteDvdCommandHandler _handler;

    public DeleteDvdCommandHandlerTests()
    {
        _handler = new DeleteDvdCommandHandler(_repositoryMock.Object);
    }

    private static Dvd ExistingDvd() =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsEmpty()
    {
        var command = new DeleteDvdCommand(Guid.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDvdDoesNotExist()
    {
        var command = new DeleteDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryUpdateFails()
    {
        var dvd = ExistingDvd();
        var command = new DeleteDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldMarkDvdAsUnavailableAndReturnResponse_WhenDvdExists()
    {
        var dvd = ExistingDvd();
        var command = new DeleteDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(dvd.Id.ToString());
        dvd.Available.Should().BeFalse();
        dvd.Copies.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDvdIsAlreadyDeleted()
    {
        var dvd = ExistingDvd();
        dvd.DeleteDvd();
        var command = new DeleteDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BuildingBlocks.Core.DomainObjects.DomainException>();
    }
}
