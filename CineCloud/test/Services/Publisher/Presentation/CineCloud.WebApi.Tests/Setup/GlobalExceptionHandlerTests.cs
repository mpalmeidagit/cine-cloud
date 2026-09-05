using System.Text;
using BuildingBlocks.Core.DomainObjects;
using CineCloud.WebApi.Setup;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CineCloud.WebApi.Tests.Setup;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler = new(NullLogger<GlobalExceptionHandler>.Instance);

    private async Task<(int StatusCode, string Body)> InvokeAsync(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        handled.Should().BeTrue();

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        return (httpContext.Response.StatusCode, body);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn500_ForArgumentNullException()
    {
        var (statusCode, body) = await InvokeAsync(new ArgumentNullException("param", "value is required"));

        statusCode.Should().Be(500);
        body.Should().Contain("value is required");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn500_ForDomainException()
    {
        var (statusCode, body) = await InvokeAsync(new DomainException("invalid state"));

        statusCode.Should().Be(500);
        body.Should().Contain("invalid state");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturn500_ForFluentValidationException()
    {
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        var (statusCode, body) = await InvokeAsync(new ValidationException(failures));

        statusCode.Should().Be(500);
        body.Should().Contain("Name is required");
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnGenericMessage_ForUnmappedException()
    {
        var (statusCode, body) = await InvokeAsync(new InvalidOperationException("boom"));

        statusCode.Should().Be(500);
        body.Should().Contain("Algo deu errado");
    }
}
