using System.Text.Json;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;
using CineCloud.WebApi.Cache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace CineCloud.WebApi.Tests.Cache;

public class CacheRepositoryTests
{
    private readonly Mock<IDistributedCache> _distributedCacheMock = new();
    private readonly CacheRepository _repository;

    public CacheRepositoryTests()
    {
        _repository = new CacheRepository(_distributedCacheMock.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnNull_WhenKeyIsNotCached()
    {
        _distributedCacheMock.Setup(c => c.GetAsync("Jaws", It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

        var result = await _repository.Get("Jaws");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Get_ShouldReturnDeserializedResponse_WhenKeyIsCached()
    {
        var response = new GetDvdResponse("1", "Jaws", "Action", DateTime.Now.AddYears(-40), 5, "director-1", DateTime.Now, DateTime.Now);
        var bytes = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        _distributedCacheMock.Setup(c => c.GetAsync("Jaws", It.IsAny<CancellationToken>())).ReturnsAsync(bytes);

        var result = await _repository.Get("Jaws");

        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.Title.Should().Be("Jaws");
    }

    [Fact]
    public async Task Update_ShouldStoreSerializedResponse_KeyedByTitle()
    {
        var response = new GetDvdResponse("1", "Jaws", "Action", DateTime.Now.AddYears(-40), 5, "director-1", DateTime.Now, DateTime.Now);

        await _repository.Update(response);

        _distributedCacheMock.Verify(c => c.SetAsync(
            "Jaws",
            It.Is<byte[]>(bytes => JsonSerializer.Deserialize<GetDvdResponse>(bytes) == response),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
