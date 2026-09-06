using CineCloud.Queries.Application.Contracts;
using CineCloud.Queries.Application.Features.Directors.Queries.GetAllDirectors;
using CineCloud.Queries.Domain.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CineCloud.Queries.Application.Tests.Features.Directors.Queries.GetAllDirectors;

public class GetAllDirectorsQueryHandlerTests
{
    private readonly Mock<IDirectorsQueryRepository> _repositoryMock = new();
    private readonly GetAllDirectorsQueryHandler _handler;

    public GetAllDirectorsQueryHandlerTests()
    {
        _handler = new GetAllDirectorsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedItemsAndPagination_WhenRepositoryReturnsData()
    {
        var directors = new List<Director>
        {
            new() { Id = "1", FullName = "George Lucas" },
            new() { Id = "2", FullName = "Steven Spielberg" }
        };
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((directors, 2L));

        var result = await _handler.Handle(new GetAllDirectorsQuery(1, 10), CancellationToken.None);

        result.Directors.Should().HaveCount(2);
        result.Directors.Should().Contain(d => d.Id == "1" && d.FullName == "George Lucas");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldNormalizePageToOne_WhenPageIsLessThanOne()
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Director>(), 0L));

        var result = await _handler.Handle(new GetAllDirectorsQuery(0, 10), CancellationToken.None);

        result.Page.Should().Be(1);
        _repositoryMock.Verify(r => r.GetAll(1, 10), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(101)]
    public async Task Handle_ShouldFallBackToDefaultPageSize_WhenPageSizeIsOutOfRange(int pageSize)
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Director>(), 0L));

        var result = await _handler.Handle(new GetAllDirectorsQuery(1, pageSize), CancellationToken.None);

        result.PageSize.Should().Be(10);
        _repositoryMock.Verify(r => r.GetAll(1, 10), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryHasNoData()
    {
        _repositoryMock.Setup(r => r.GetAll(1, 10)).ReturnsAsync((new List<Director>(), 0L));

        var result = await _handler.Handle(new GetAllDirectorsQuery(1, 10), CancellationToken.None);

        result.Directors.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
