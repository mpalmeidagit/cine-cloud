using BuildingBlocks.Core.DomainObjects;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Core.Tests.DomainObjects;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        var exception = new DomainException("invalid state");

        exception.Message.Should().Be("invalid state");
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        var inner = new InvalidOperationException("root cause");

        var exception = new DomainException("invalid state", inner);

        exception.Message.Should().Be("invalid state");
        exception.InnerException.Should().Be(inner);
    }

    [Fact]
    public void DomainException_ShouldBeAnException()
    {
        var exception = new DomainException("invalid state");

        exception.Should().BeAssignableTo<Exception>();
    }
}
