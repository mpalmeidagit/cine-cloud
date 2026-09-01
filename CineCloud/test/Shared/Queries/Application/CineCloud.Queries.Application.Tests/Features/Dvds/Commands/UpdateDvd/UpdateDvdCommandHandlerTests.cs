using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly UpdateDvdCommandHandler _handler;

    public UpdateDvdCommandHandlerTests()
    {
        _handler = new UpdateDvdCommandHandler(_repositoryMock.Object, new UpdateDvdCommandValidator());
    }

    private static UpdateDvdCommand ValidCommand(string? id = null) =>
        new(id ?? Guid.NewGuid().ToString(), "Jaws 2", "Adventure", DateTime.Now.AddYears(-30), 3,
            Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
    {
        var command = ValidCommand() with { Title = "" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDvdDoesNotExist()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Dvd>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllFieldsAndReturnTrue_WhenCommandIsValid()
    {
        var command = ValidCommand();
        var dvd = new Dvd { Id = command.Id, Title = "Old", Genre = "Old", DirectorId = "old-director" };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        dvd.Title.Should().Be(command.Title);
        dvd.Genre.Should().Be(command.Genre);
        dvd.Copies.Should().Be(command.Copies);
        dvd.DirectorId.Should().Be(command.DirectorId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryUpdateFails()
    {
        var command = ValidCommand();
        var dvd = new Dvd { Id = command.Id, Title = "Old", Genre = "Old", DirectorId = "old-director" };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(dvd);
        _repositoryMock.Setup(r => r.Update(dvd)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
