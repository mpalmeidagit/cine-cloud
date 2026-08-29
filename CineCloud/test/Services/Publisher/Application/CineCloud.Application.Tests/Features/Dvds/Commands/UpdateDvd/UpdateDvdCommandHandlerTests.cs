using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Dvds.Commands.UpdateDvd;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandlerTests
{
    private readonly Mock<IDvdsWriteRepository> _repositoryMock = new();
    private readonly UpdateDvdCommandHandler _handler;

    public UpdateDvdCommandHandlerTests()
    {
        _handler = new UpdateDvdCommandHandler(_repositoryMock.Object, new UpdateDvdCommandValidator());
    }

    private static Dvd ExistingDvd() =>
        new("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenCommandIsInvalid()
    {
        var command = new UpdateDvdCommand(Guid.Empty, "Jaws", 0, DateTime.Now.AddYears(-40), Guid.NewGuid(), 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDvdDoesNotExist()
    {
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 0, DateTime.Now.AddYears(-30), Guid.NewGuid(), 3);
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Dvd>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryUpdateFails()
    {
        var dvd = ExistingDvd();
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 1, DateTime.Now.AddYears(-30), Guid.NewGuid(), 3);
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllFieldsAndReturnResponse_WhenCommandIsValid()
    {
        var dvd = ExistingDvd();
        var newDirectorId = Guid.NewGuid();
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 1, DateTime.Now.AddYears(-30), newDirectorId, 3);
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Jaws 2");
        result.Genre.Should().Be("Adventure");
        result.Copies.Should().Be(3);
        result.DirectorId.Should().Be(newDirectorId.ToString());
        dvd.Title.Should().Be("Jaws 2");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenDvdIsNotAvailable()
    {
        var dvd = ExistingDvd();
        dvd.DeleteDvd();
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 1, DateTime.Now.AddYears(-30), Guid.NewGuid(), 3);
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BuildingBlocks.Core.DomainObjects.DomainException>();
    }
}
