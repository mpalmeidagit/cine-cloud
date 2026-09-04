using BuildingBlocks.Core;
using BuildingBlocks.Core.Mediator;
using CineCloud.Application.Features.Dvds.Commands.CreateDvd;
using CineCloud.Application.Features.Dvds.Commands.DeleteDvd;
using CineCloud.Application.Features.Dvds.Commands.RentDvd;
using CineCloud.Application.Features.Dvds.Commands.ReturnDvd;
using CineCloud.Application.Features.Dvds.Commands.UpdateDvd;
using CineCloud.Queries.Application.Features.Dvds.Queries.GetDvd;
using CineCloud.WebApi.Cache;
using CineCloud.WebApi.Controllers;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CineCloud.WebApi.Tests.Controllers;

public class DvdsControllerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly Mock<ICacheRepository> _cacheRepositoryMock = new();
    private readonly DvdsController _controller;

    public DvdsControllerTests()
    {
        _controller = new DvdsController(_mediatorMock.Object, _publishEndpointMock.Object, _cacheRepositoryMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetDvd_ShouldReturnCachedResponse_WhenCacheHit()
    {
        var cached = new GetDvdResponse("1", "Jaws", "Action", DateTime.Now, 5, "d1", DateTime.Now, DateTime.Now);
        _cacheRepositoryMock.Setup(c => c.Get("Jaws")).ReturnsAsync(cached);

        var result = await _controller.GetDvd("Jaws");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(cached);
        _mediatorMock.Verify(m => m.SendQuery(It.IsAny<GetDvdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetDvd_ShouldQueryAndCache_WhenCacheMissAndDvdExists()
    {
        _cacheRepositoryMock.Setup(c => c.Get("Jaws")).ReturnsAsync((GetDvdResponse)null!);
        var response = new GetDvdResponse("1", "Jaws", "Action", DateTime.Now, 5, "d1", DateTime.Now, DateTime.Now);
        _mediatorMock.Setup(m => m.SendQuery(new GetDvdQuery("Jaws"), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.GetDvd("Jaws");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
        _cacheRepositoryMock.Verify(c => c.Update(response), Times.Once);
    }

    [Fact]
    public async Task GetDvd_ShouldReturnNotFound_WhenCacheMissAndDvdDoesNotExist()
    {
        _cacheRepositoryMock.Setup(c => c.Get("Jaws")).ReturnsAsync((GetDvdResponse)null!);
        _mediatorMock.Setup(m => m.SendQuery(new GetDvdQuery("Jaws"), It.IsAny<CancellationToken>())).ReturnsAsync((GetDvdResponse)null!);

        var result = await _controller.GetDvd("Jaws");

        result.Should().BeOfType<NotFoundObjectResult>();
        _cacheRepositoryMock.Verify(c => c.Update(It.IsAny<GetDvdResponse>()), Times.Never);
    }

    [Fact]
    public async Task CreateDvd_ShouldReturnCreated_WhenMediatorReturnsResponse()
    {
        var command = new CreateDvdCommand("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());
        var response = new CreateDvdResponse(Guid.NewGuid().ToString(), "Jaws", "Action", command.Published, true, 5, command.DirectorId.ToString(), DateTime.Now, DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.CreateDvd(command);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
    }

    [Fact]
    public async Task CreateDvd_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var command = new CreateDvdCommand("Jaws", 0, DateTime.Now.AddYears(-40), 5, Guid.NewGuid());
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.CreateDvd(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateDvd_ShouldReturnOk_WhenMediatorReturnsResponse()
    {
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 1, DateTime.Now.AddYears(-30), Guid.NewGuid(), 3);
        var response = new UpdateDvdResponse(command.Id.ToString(), "Jaws 2", "Adventure", command.Published, 3, command.DirectorId.ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.UpdateDvd(command);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
    }

    [Fact]
    public async Task UpdateDvd_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var command = new UpdateDvdCommand(Guid.NewGuid(), "Jaws 2", 1, DateTime.Now.AddYears(-30), Guid.NewGuid(), 3);
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.UpdateDvd(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RentDvd_ShouldReturnOk_WhenMediatorReturnsResponse()
    {
        var id = Guid.NewGuid();
        var response = new RentDvdResponse(id.ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(new RentDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.RentDvd(id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RentDvd_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendCommand(new RentDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.RentDvd(id);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ReturnDvd_ShouldReturnOk_WhenMediatorReturnsResponse()
    {
        var id = Guid.NewGuid();
        var response = new ReturnDvdResponse(id.ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(new ReturnDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.ReturnDvd(id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ReturnDvd_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendCommand(new ReturnDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.ReturnDvd(id);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteDvd_ShouldReturnOk_WhenMediatorReturnsResponse()
    {
        var id = Guid.NewGuid();
        var response = new DeleteDvdResponse(id.ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(new DeleteDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.DeleteDvd(id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteDvd_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendCommand(new DeleteDvdCommand(id), It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.DeleteDvd(id);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
