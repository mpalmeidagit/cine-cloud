using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetAllDvds;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Dvds.Queries.GetAllDvds;

public class GetAllDvdsQueryHandlerTests
{
    private readonly Mock<IDvdsQueryRepository> _repositoryMock = new();
    private readonly GetAllDvdsQueryHandler _handler;

    public GetAllDvdsQueryHandlerTests()
    {
        _handler = new GetAllDvdsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedItemsAndPagination_WhenRepositoryReturnsData()
    {
        var dvds = new List<Dvd>
        {
            new() { Id = "1", Title = "Jaws", Genre = "Action", Copies = 5, DirectorId = "d1" },
            new() { Id = "2", Title = "E.T.", Genre = "Family", Copies = 3, DirectorId = "d1" }
        };
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((dvds, 2L));

        var result = await _handler.Handle(new GetAllDvdsQuery(1, 10), CancellationToken.None);

        result.Dvds.Should().HaveCount(2);
        result.Dvds.Should().Contain(d => d.Id == "1" && d.Title == "Jaws" && d.Copies == 5);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldNormalizePageToOne_WhenPageIsLessThanOne()
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Dvd>(), 0L));

        var result = await _handler.Handle(new GetAllDvdsQuery(-1, 10), CancellationToken.None);

        result.Page.Should().Be(1);
        _repositoryMock.Verify(r => r.GetAll(1, 10), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(101)]
    public async Task Handle_ShouldFallBackToDefaultPageSize_WhenPageSizeIsOutOfRange(int pageSize)
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Dvd>(), 0L));

        var result = await _handler.Handle(new GetAllDvdsQuery(1, pageSize), CancellationToken.None);

        result.PageSize.Should().Be(10);
        _repositoryMock.Verify(r => r.GetAll(1, 10), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryHasNoData()
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Dvd>(), 0L));

        var result = await _handler.Handle(new GetAllDvdsQuery(1, 10), CancellationToken.None);

        result.Dvds.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
