using BuildingBlocks.Core;
using CineCloud.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CineCloud.WebApi.Tests.Controllers;

public class ApiControllerTests
{
    private sealed class TestableApiController : ApiController
    {
        public ActionResult Invoke(int status, bool success, object? data = null) =>
            CustomResponse(status, success, data);
    }

    private readonly TestableApiController _controller = new();

    [Fact]
    public void CustomResponse_ShouldReturnNotFound_ForNotFoundFailure()
    {
        var result = _controller.Invoke(404, false);

        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().BeOfType<BaseResponse>()
            .Which.Message.Should().Be("No elements found.");
    }

    [Fact]
    public void CustomResponse_ShouldReturnBadRequest_ForBadRequestFailure()
    {
        var result = _controller.Invoke(400, false);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().BeOfType<BaseResponse>()
            .Which.Message.Should().Be("Errors during the transaction.");
    }

    [Fact]
    public void CustomResponse_ShouldReturnOkWithCreatedMessage_ForCreatedSuccess()
    {
        var data = new { Id = "abc" };

        var result = _controller.Invoke(201, true, data);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse>().Subject;
        body.Message.Should().Be("Created");
        body.Data.Should().Be(data);
    }

    [Fact]
    public void CustomResponse_ShouldReturnOkWithData_ForOkSuccess()
    {
        var data = new { Id = "abc" };

        var result = _controller.Invoke(200, true, data);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse>().Which.Data.Should().Be(data);
    }

    [Theory]
    [InlineData(500, false)]
    [InlineData(404, true)]
    [InlineData(200, false)]
    public void CustomResponse_ShouldFallBackToStatusCode_ForUnmappedCombinations(int status, bool success)
    {
        var result = _controller.Invoke(status, success);

        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(status);
        statusResult.Value.Should().BeOfType<BaseResponse>()
            .Which.Success.Should().Be(success);
    }
}
