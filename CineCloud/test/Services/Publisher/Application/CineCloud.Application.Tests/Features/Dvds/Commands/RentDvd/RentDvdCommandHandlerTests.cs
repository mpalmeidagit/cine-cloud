using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Dvds.Commands.RentDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.RentDvd;

public class RentDvdCommandHandlerTests
{
    private readonly Mock<IDvdsWriteRepository> _repositoryMock = new();
    private readonly RentDvdCommandHandler _handler;

    public RentDvdCommandHandlerTests()
    {
        _handler = new RentDvdCommandHandler(_repositoryMock.Object);
    }

    private static Dvd ExistingDvd(int copies = 5) =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), copies, Guid.NewGuid());

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenIdIsEmpty()
    {
        var command = new RentDvdCommand(Guid.Empty);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDvdDoesNotExist()
    {
        var command = new RentDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryUpdateFails()
    {
        var dvd = ExistingDvd();
        var command = new RentDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldDecrementCopiesAndReturnResponse_WhenDvdIsAvailable()
    {
        var dvd = ExistingDvd(copies: 5);
        var command = new RentDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(dvd.Id.ToString());
        dvd.Copies.Should().Be(4);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoCopiesAreLeft()
    {
        var dvd = ExistingDvd(copies: 0);
        var command = new RentDvdCommand(Guid.NewGuid());
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BuildingBlocks.Core.DomainObjects.DomainException>();
    }
}
