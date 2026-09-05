using BuildingBlocks.Core;
using BuildingBlocks.Core.Mediator;
using CineCloud.Application.Features.Directors.Commands.CreateDirector;
using CineCloud.Application.Features.Directors.Commands.DeleteDirector;
using CineCloud.Application.Features.Directors.Commands.UpdateDirector;
using CineCloud.Queries.Application.Features.Directors.Queries.GetDirector;
using CineCloud.WebApi.Controllers;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CineCloud.WebApi.Tests.Controllers;

public class DirectorsControllerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly Mock<IPublishEndpoint> _publishEndpointMock = new();
    private readonly DirectorsController _controller;

    public DirectorsControllerTests()
    {
        _controller = new DirectorsController(_mediatorMock.Object, _publishEndpointMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task GetDirector_ShouldReturnOk_WhenDirectorExists()
    {
        var response = new GetDirectorResponse("1", "Steven Spielberg");
        _mediatorMock.Setup(m => m.SendQuery(new GetDirectorQuery("Steven Spielberg"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetDirector("Steven Spielberg");

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
    }

    [Fact]
    public async Task GetDirector_ShouldReturnNotFound_WhenDirectorDoesNotExist()
    {
        _mediatorMock.Setup(m => m.SendQuery(new GetDirectorQuery("Steven Spielberg"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetDirectorResponse)null!);

        var result = await _controller.GetDirector("Steven Spielberg");

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateDirector_ShouldReturnCreated_WhenMediatorReturnsResponse()
    {
        var command = new CreateDirectorCommand("Steven", "Spielberg");
        var response = new CreateDirectorResponse(Guid.NewGuid().ToString(), "Steven Spielberg", DateTime.Now, DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.CreateDirector(command);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
        _publishEndpointMock.Verify(p => p.Publish(It.IsAny<BuildingBlocks.Core.EventBus.Events.DirectorCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDirector_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var command = new CreateDirectorCommand("Steven", "Spielberg");
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.CreateDirector(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _publishEndpointMock.Verify(p => p.Publish(It.IsAny<BuildingBlocks.Core.EventBus.Events.DirectorCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDirector_ShouldReturnOk_WhenMediatorReturnsResponse()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "George", "Lucas");
        var response = new UpdateDirectorResponse(command.Id.ToString(), "George Lucas", DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.UpdateDirector(command);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(response);
    }

    [Fact]
    public async Task UpdateDirector_ShouldReturnBadRequest_WhenMediatorReturnsNull()
    {
        var command = new UpdateDirectorCommand(Guid.NewGuid(), "George", "Lucas");
        _mediatorMock.Setup(m => m.SendCommand(command, It.IsAny<CancellationToken>())).ReturnsAsync((IResponse)null!);

        var result = await _controller.UpdateDirector(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteDirector_ShouldReturnOk_WhenMediatorReturnsTrue()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(new DeleteDirectorCommand(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteDirector(id);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteDirector_ShouldReturnBadRequest_WhenMediatorReturnsFalse()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(new DeleteDirectorCommand(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.DeleteDirector(id);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
