using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Dvds;
using CineCloud.Queries.Application.Features.Dvds.Commands.RentDvd;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Dvds;

public class DvdRentedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DvdRentedConsumer _consumer;

    public DvdRentedConsumerTests()
    {
        _consumer = new DvdRentedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DvdRentedConsumer>>());
    }

    private static Mock<ConsumeContext<DvdRentedEvent>> ContextFor(DvdRentedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DvdRentedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendRentDvdCommand_WhenEventIsValid()
    {
        var @event = new DvdRentedEvent(Guid.NewGuid().ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<RentDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<RentDvdCommand>(c => c.Id == @event.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = new DvdRentedEvent(Guid.NewGuid().ToString(), DateTime.Now);
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<RentDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenIdIsEmpty()
    {
        var @event = new DvdRentedEvent("", DateTime.Now);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(It.IsAny<RentDvdCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
