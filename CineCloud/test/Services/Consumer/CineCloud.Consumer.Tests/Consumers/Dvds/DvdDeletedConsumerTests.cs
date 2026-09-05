using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Dvds;
using CineCloud.Queries.Application.Features.Dvds.Commands.DeleteDvd;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Dvds;

public class DvdDeletedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DvdDeletedConsumer _consumer;

    public DvdDeletedConsumerTests()
    {
        _consumer = new DvdDeletedConsumer(Mock.Of<ILogger<DvdDeletedConsumer>>(), _mediatorMock.Object);
    }

    private static Mock<ConsumeContext<DvdDeletedEvent>> ContextFor(DvdDeletedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DvdDeletedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendDeleteDvdCommand_WhenEventIsValid()
    {
        var @event = new DvdDeletedEvent(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<DeleteDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<DeleteDvdCommand>(c => c.Id == @event.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DvdDeletedEvent(Guid.NewGuid().ToString(), DateTime.Now.AddMinutes(-1));
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<DeleteDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenDeletedAtIsInTheFuture()
    {
        var @event = new DvdDeletedEvent(Guid.NewGuid().ToString(), DateTime.Now.AddDays(1));

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(It.IsAny<DeleteDvdCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
