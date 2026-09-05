using BuildingBlocks.Core.EventBus.Events;
using BuildingBlocks.Core.Mediator;
using CineCloud.Consumer.Consumers.Dvds;
using CineCloud.Queries.Application.Features.Dvds.Commands.UpdateDvd;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CineCloud.Consumer.Tests.Consumers.Dvds;

public class DvdUpdatedConsumerTests
{
    private readonly Mock<IMediatorHandler> _mediatorMock = new();
    private readonly DvdUpdatedConsumer _consumer;

    public DvdUpdatedConsumerTests()
    {
        _consumer = new DvdUpdatedConsumer(_mediatorMock.Object, Mock.Of<ILogger<DvdUpdatedConsumer>>());
    }

    private static DvdUpdatedEvent ValidEvent() => new(
        Guid.NewGuid().ToString(), "Jaws 2", "Adventure", DateTime.Now.AddYears(-30), 3,
        Guid.NewGuid().ToString(), DateTime.Now);

    private static Mock<ConsumeContext<DvdUpdatedEvent>> ContextFor(DvdUpdatedEvent @event)
    {
        var contextMock = new Mock<ConsumeContext<DvdUpdatedEvent>>();
        contextMock.Setup(c => c.Message).Returns(@event);
        return contextMock;
    }

    [Fact]
    public async Task Consume_ShouldSendUpdateDvdCommand_WhenEventIsValid()
    {
        var @event = ValidEvent();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<UpdateDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(ContextFor(@event).Object);

        _mediatorMock.Verify(m => m.SendCommandAndReturnBool(
            It.Is<UpdateDvdCommand>(c => c.Id == @event.Id && c.Title == @event.Title),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldThrow_WhenMediatorReturnsFalse()
    {
        var @event = ValidEvent();
        _mediatorMock.Setup(m => m.SendCommandAndReturnBool(It.IsAny<UpdateDvdCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _consumer.Consume(ContextFor(@event).Object);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
