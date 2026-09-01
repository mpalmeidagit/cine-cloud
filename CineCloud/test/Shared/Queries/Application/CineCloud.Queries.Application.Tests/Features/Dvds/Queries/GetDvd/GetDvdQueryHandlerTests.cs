using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Queries.GetDvd;

public class GetDvdQueryHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly GetDvdQueryHandler _handler;

    public GetDvdQueryHandlerTests()
    {
        _handler = new GetDvdQueryHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Handle_ShouldReturnNull_WhenTitleIsEmpty(string? title)
    {
        var query = new GetDvdQuery(title!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _repositoryMock.Verify(r => r.GetByTitle(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenDvdDoesNotExist()
    {
        var query = new GetDvdQuery("Jaws");
        _repositoryMock.Setup(r => r.GetByTitle(query.Title)).ReturnsAsync((Dvd)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_WhenDvdExists()
    {
        var query = new GetDvdQuery("Jaws");
        var dvd = new Dvd { Id = "1", Title = "Jaws", Genre = "Action", Copies = 5, DirectorId = "d1" };
        _repositoryMock.Setup(r => r.GetByTitle(query.Title)).ReturnsAsync(dvd);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.Title.Should().Be("Jaws");
        result.Genre.Should().Be("Action");
        result.Copies.Should().Be(5);
        result.DirectorId.Should().Be("d1");
    }
}
