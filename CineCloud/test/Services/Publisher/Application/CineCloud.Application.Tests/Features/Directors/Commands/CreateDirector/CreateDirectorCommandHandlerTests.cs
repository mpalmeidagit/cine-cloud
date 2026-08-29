using CineCloud.Application.Contracts;
using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Application.Tests.Features.Directors.Commands.CreateDirector;

public class CreateDirectorCommandHandlerTests
{
    private readonly Mock<IDirectorsWriteRepository> _repositoryMock = new();
    private readonly CreateDirectorCommandHandler _handler;

    public CreateDirectorCommandHandlerTests()
    {
        _handler = new CreateDirectorCommandHandler(_repositoryMock.Object, new CreateDirectorCommandValidator());
    }

    [Fact]
    public async Task Handle_ShouldReturnNullAndNotCreate_WhenCommandIsInvalid()
    {
        var command = new CreateDirectorCommand("", "Spielberg");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.Create(It.IsAny<Director>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_WhenCommandIsValid()
    {
        var command = new CreateDirectorCommand("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.Create(It.IsAny<Director>())).ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullName.Should().Be("Steven Spielberg");
        _repositoryMock.Verify(r => r.Create(It.Is<Director>(d => d.Name == "Steven" && d.Surname == "Spielberg")), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenRepositoryCreateFails()
    {
        var command = new CreateDirectorCommand("Steven", "Spielberg");
        _repositoryMock.Setup(r => r.Create(It.IsAny<Director>())).ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
