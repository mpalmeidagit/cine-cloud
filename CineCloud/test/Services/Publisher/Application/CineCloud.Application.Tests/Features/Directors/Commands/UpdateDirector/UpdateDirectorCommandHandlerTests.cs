using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Directors.Commands.UpdateDirector;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsWriteRepository> _repositoryMock = new();
    private readonly UpdateDirectorCommandHandler _handler;

    public UpdateDirectorCommandHandlerTests()
    {
        _handler = new UpdateDirectorCommandHandler(_repositoryMock.Object, new UpdateDirectorCommandValidator());
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenCommandIsInvalid()
    {
        var command = new UpdateDirectorCommand(Guid.Empty, "Steven", "Spielberg");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Get(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDirectorDoesNotExist()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "Steven", "Spielberg");
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Director>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryUpdateFails()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "George", "Lucas");
        var director = new Director("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Update(director)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldUpdateDirectorAndReturnResponse_WhenCommandIsValid()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "George", "Lucas");
        var director = new Director("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Update(director)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullName.Should().Be("George Lucas");
        director.Name.Should().Be("George");
        director.Surname.Should().Be("Lucas");
    }
}
