using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Commands.UpdateDirector;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsQueryRepository> _repositoryMock = new();
    private readonly UpdateDirectorCommandHandler _handler;

    public UpdateDirectorCommandHandlerTests()
    {
        _handler = new UpdateDirectorCommandHandler(_repositoryMock.Object, new UpdateDirectorCommandValidator());
    }

    private static UpdateDirectorCommand ValidCommand(string? id = null) =>
        new(id ?? Guid.NewGuid().ToString(), "George Lucas", DateTime.Now.AddMinutes(-1));

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
    {
        var command = ValidCommand() with { FullName = "" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDirectorDoesNotExist()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Director>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDirectorAndReturnTrue_WhenCommandIsValid()
    {
        var command = ValidCommand();
        var director = new Director { Id = command.Id, FullName = "Old Name" };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Update(director)).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        director.FullName.Should().Be(command.FullName);
        director.UpdatedAt.Should().Be(command.UpdatedAt);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryUpdateFails()
    {
        var command = ValidCommand();
        var director = new Director { Id = command.Id, FullName = "Old Name" };
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(director);
        _repositoryMock.Setup(r => r.Update(director)).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }
}
