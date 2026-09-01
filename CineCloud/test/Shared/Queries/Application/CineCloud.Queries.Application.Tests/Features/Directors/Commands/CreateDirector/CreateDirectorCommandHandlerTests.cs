using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Commands.CreateDirector;

public class CreateDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsQueryRepository> _repositoryMock = new();
    private readonly CreateDirectorCommandHandler _handler;

    public CreateDirectorCommandHandlerTests()
    {
        _handler = new CreateDirectorCommandHandler(_repositoryMock.Object, new CreateDirectorCommandValidator());
    }

    private static CreateDirectorCommand ValidCommand(string? id = null) =>
        new(id ?? Guid.NewGuid().ToString(), "Steven Spielberg", DateTime.Now.AddMinutes(-1), DateTime.Now.AddMinutes(-1));

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
    {
        var command = ValidCommand() with { FullName = "" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Get(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDirectorAlreadyExists()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync(new Director { Id = command.Id, FullName = "Existing" });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _repositoryMock.Verify(r => r.Create(It.IsAny<Director>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryCreateReturnsNull()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Director)null!);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Director>())).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenDirectorIsCreated()
    {
        var command = ValidCommand();
        _repositoryMock.Setup(r => r.Get(command.Id)).ReturnsAsync((Director)null!);
        _repositoryMock.Setup(r => r.Create(It.IsAny<Director>())).ReturnsAsync(new Director { Id = command.Id, FullName = command.FullName });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.Create(It.Is<Director>(d => d.Id == command.Id && d.FullName == command.FullName)), Times.Once);
    }
}
