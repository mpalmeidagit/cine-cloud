using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Dvds;
using CineCloud.Queries.Application.Features.Dvds.Commands.ReturnDvd;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Dvds;

public class DvdReturnedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DvdReturnedConsumer _consumer;

    public DvdReturnedConsumerTests()
    {
        _consumer = new DvdReturnedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DvdReturnedConsumer>>());
    }

    private static Mock<ConsumeContext<DvdReturnedEvent>> ContextFor(DvdReturnedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DvdReturnedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendReturnDvdCommand_WhenEventIsValid()
    {
        var @event = new DvdReturnedEvent(Guid.NewGuid().ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<ReturnDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<ReturnDvdCommand>(c => c.Id == @event.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DvdReturnedEvent(Guid.NewGuid().ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<ReturnDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenIdIsEmpty()
    {
        var @event = new DvdReturnedEvent("", DateTime.Now);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(It.IsAny<ReturnDvdCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
