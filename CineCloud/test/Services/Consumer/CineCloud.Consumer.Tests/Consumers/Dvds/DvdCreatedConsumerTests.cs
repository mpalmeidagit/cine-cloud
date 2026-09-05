using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Dvds;
using CineCloud.Queries.Application.Features.Dvds.Commands.CreateDvd;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Dvds;

public class DvdCreatedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DvdCreatedConsumer _consumer;

    public DvdCreatedConsumerTests()
    {
        _consumer = new DvdCreatedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DvdCreatedConsumer>>());
    }

    private static DvdCreatedEvent ValidEvent() => new(
        Guid.NewGuid().ToString(), "Jaws", "Action", DateTime.Now.AddYears(-40), true, 5,
        Guid.NewGuid().ToString(), DateTime.Now, DateTime.Now);

    private static Mock<ConsumeContext<DvdCreatedEvent>> ContextFor(DvdCreatedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DvdCreatedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendCreateDvdCommand_WhenEventIsValid()
    {
        var @event = ValidEvent();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<CreateDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<CreateDvdCommand>(c => c.Id == @event.Id && c.Title == @event.Title),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = ValidEvent();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<CreateDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
