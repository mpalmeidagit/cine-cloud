using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Queries.GetDirector;

public class GetDirectorQueryHandlerTests
{
    private readonly Mock<IDirectorsQueryRepository> _repositoryMock = new();
    private readonly GetDirectorQueryHandler _handler;

    public GetDirectorQueryHandlerTests()
    {
        _handler = new GetDirectorQueryHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldReturnNull_WhenFullNameIsEmpty(string? fullName)
    {
        var query = new GetDirectorQuery(fullName!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDirectorDoesNotExist()
    {
        var query = new GetDirectorQuery("Steven Spielberg");
        _repositoryMock.Setup(r => r.GetByName(query.FullName)).ReturnsAsync((Director)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_WhenDirectorExists()
    {
        var query = new GetDirectorQuery("Steven Spielberg");
        var director = new Director { Id = "1", FullName = "Steven Spielberg" };
        _repositoryMock.Setup(r => r.GetByName(query.FullName)).ReturnsAsync(director);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.FullName.Should().Be("Steven Spielberg");
    }
}
